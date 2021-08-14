using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace EfSqlServer.Docker
{
    public class TestSqlHelper
    {
        private readonly string _connectionString;

        public TestSqlHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public async Task<bool> DoesTableExist(string tableSchema, string tableName)
        {
            var sql = $@"SELECT CASE WHEN EXISTS(SELECT * FROM information_schema.tables WHERE TABLE_SCHEMA = '{tableSchema}' AND  TABLE_NAME = '{tableName}') THEN 1 ELSE 0 END";
            return (await RunSqlThatReturnsInteger(sql))== 1;
        }

        public async Task<bool> DoesTableHaveThisManyRows(string tableName, int rowCount)
        {
            var sql = $@"SELECT COUNT(1) FROM {tableName}";
            return (await RunSqlThatReturnsInteger(sql)) == rowCount;
        }

        public async Task<int> RunSqlThatReturnsInteger(string sql)
        {
            await using var targetConnection = new SqlConnection(_connectionString);
            targetConnection.Open();
            var command = new SqlCommand(sql,
                targetConnection);
            var sqlResult = await command.ExecuteScalarAsync();
            var result = (int)sqlResult;
            return result;
        }
        
        public async Task<DateTime?> RunSqlThatReturnsDateTime(string sql)
        {
            await using var targetConnection = new SqlConnection(_connectionString);
            targetConnection.Open();
            var command = new SqlCommand(sql,
                targetConnection);
            var sqlResult = await command.ExecuteScalarAsync();
            var result = sqlResult as DateTime?;
            return result;
        }

        public async Task<string> RunSqlThatReturnsString(string sql)
        {
            await using var targetConnection = new SqlConnection(_connectionString);
            targetConnection.Open();
            var command = new SqlCommand(sql,
                targetConnection);
            var sqlResult = await command.ExecuteScalarAsync();
            var result = sqlResult.ToString();
            return result;
        }

        public async Task<int> RunSqlCommand(string sql)
        {
            await using var targetConnection = new SqlConnection(_connectionString);
            targetConnection.Open();
            var command = new SqlCommand(sql,
                targetConnection);
            var sqlResult = await command.ExecuteNonQueryAsync();
            return sqlResult;
        }
    }
}
