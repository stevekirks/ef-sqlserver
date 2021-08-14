using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Data.SqlClient;
using Serilog;

namespace EfSqlServer.Docker
{
    public class DockerSetup : IDisposable
    {
        // These are specified in the docker-compose.yml file
        private const string SqlServerDbName = "testdb";
        private const int SqlServerDbPort = 41707;
        private const string SqlServerSaPassword = "thisStrong(!)Password";
        private const string DockerFolder = "Docker";

        public void RunDockerContainer()
        {
            var proc = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "docker-compose",
                    Arguments = "up -d",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = DockerFolder
                });

            // wait max 5 minutes - it can take a little while to download the image to the local cache
            proc.WaitForExit(1000 * 60 * 5);

            if (proc.ExitCode != 0)
            {
                Console.WriteLine("Failed to start docker containers");
                Console.WriteLine("StdOut:");
                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                Console.WriteLine("StdErr:");
                Console.WriteLine(proc.StandardError.ReadToEnd());
                throw new Exception("Failed to start docker containers");
            }

            Log.Debug("Containers running");
        }

        public void SetupDatabase()
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    retryCount++;
                    CreateDatabase();
                    break;
                }
                catch (SqlException e)
                {
                    if (retryCount < 8 &&
                        (e.Message.Contains("A connection was successfully established with the server, but then an error occurred during the pre-login handshake")
                        || e.Message.Contains("Login failed for user"))
                    )
                    {
                        // Need to allow for SQL Server application to be ready
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var databasePopulation = new TestDatabasePopulation(GetDbConnectionString());
            retryCount = 0;
            while (true)
            {
                try
                {
                    retryCount++;
                    databasePopulation.CreateTablesAndData();
                    break;
                }
                catch (SqlException e)
                {
                    if (retryCount < 5 && e.Message.Contains("Login failed for user"))
                    {
                        // Need to allow for SQL Server application to be ready
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private void RemoveDockerContainer()
        {
            var proc = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "docker-compose",
                    Arguments = "down",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = DockerFolder
                });

            proc.WaitForExit(1000 * 60);
        }

        public string GetDbConnectionString()
        {
            return GetDbConnectionString(SqlServerDbName, SqlServerDbPort);
        }
        
        private void CreateDatabase()
        {
            Log.Debug("Connecting to SQL Server and creating database");
            var masterDbConnectionString = GetDbConnectionString("master", SqlServerDbPort);
            var connectionString = masterDbConnectionString;
            var databaseName = SqlServerDbName;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var checkQuery = new SqlCommand($"SELECT count(*) FROM master.sys.databases where name = '{databaseName}'", connection);
            var exists = (int)checkQuery.ExecuteScalar();
            if (exists == 1)
            {
                var dropCommand = new SqlCommand($"DROP DATABASE {databaseName}", connection);
                dropCommand.ExecuteNonQuery();
            }
            var command = new SqlCommand($"CREATE DATABASE {databaseName}", connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        private string GetDbConnectionString(string databaseName, int port)
        {
            var connectionString =
                $"Server=tcp:localhost,{port};Database={databaseName};User ID=sa;Password={SqlServerSaPassword};Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;MultipleActiveResultSets=True;";
            return connectionString;
        }

        public void Dispose()
        {
            //_dockerSetup.RemoveDockerContainer();
        }
    }
}
