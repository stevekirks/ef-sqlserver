using System;
using System.IO;
using Microsoft.Data.SqlClient;

namespace EfSqlServer.Docker
{
    public class TestDatabasePopulation
    {
        private readonly string _dbConnectionString;

        public TestDatabasePopulation(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }
        
        public void CreateTablesAndData()
        {
            var connectionString = _dbConnectionString;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            RunScriptInFile(connection, "Docker/Scripts/Schema/1_CreateTable_Food.sql");
            RunScriptInFile(connection, "Docker/Scripts/Data/Food_data.sql");
            connection.Close();
        }
        
        private void RunScriptInFile(SqlConnection connection, string filename)
        {
            var sql = File.ReadAllText(filename);
            var dropCommand = new SqlCommand(sql, connection);
            dropCommand.ExecuteNonQuery();
        }
    }
}
