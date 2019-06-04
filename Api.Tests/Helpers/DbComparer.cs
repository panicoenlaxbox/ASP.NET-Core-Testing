using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

// ReSharper disable PossibleMultipleEnumeration

namespace Api.Tests.Helpers
{
    public class DbComparer
    {
        private readonly DbComparerOptions _options;

        public DbComparer(DbComparerOptions options)
        {
            _options = options;
        }

        public async Task<DbComparerResult> CompareAsync()
        {
            var result = new DbComparerResult();
            var tables = await GetSourceTablesAsync();
            foreach (var (schema, table) in tables)
            {
                result.Entries.Add(await CreateEntryAsync(schema, table));
            }

            if (_options.Count)
            {
                foreach (var entry in result.Entries.Where(e => !e.Match))
                {
                    entry.SourceCount = await GetCountAsync(_options.SourceConnectionString, entry.Schema, entry.Table);
                    entry.TargetCount = await GetCountAsync(_options.TargetConnectionString, entry.Schema, entry.Table);
                }
            }

            return result;
        }

        private static async Task<int> GetCountAsync(string connectionString, string schema, string table)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return await connection.ExecuteScalarAsync<int>(
                    $"SELECT COUNT(*) FROM [{schema}].[{table}]");
            }
        }

        private async Task<DbComparerEntryResult> CreateEntryAsync(string schema, string table)
        {
            // https://stackoverflow.com/questions/1560306/calculate-hash-or-checksum-for-a-table-in-sql-server
            // https://stackoverflow.com/questions/11994430/what-conditions-cause-checksum-agg-to-return-0

            var sql = "SELECT SUM(CAST(CHECKSUM(";
            var columns = await GetSourceColumnsAsync(schema, table);
            foreach (var column in columns)
            {
                sql += $"[{column}],";
            }
            sql = sql.TrimEnd(',') +
                  $@") AS BIGINT)) FROM [{schema}].[{table}]";
            var entry = new DbComparerEntryResult
            {
                Schema = schema,
                Table = table
            };
            using (var connection = new SqlConnection(_options.SourceConnectionString))
            {
                entry.SourceChecksum = await connection.ExecuteScalarAsync<long>(sql);
            }
            using (var connection = new SqlConnection(_options.TargetConnectionString))
            {
                entry.TargetChecksum = await connection.ExecuteScalarAsync<long>(sql);
            }
            return entry;
        }

        private async Task<IEnumerable<(string, string)>> GetSourceTablesAsync()
        {
            var sql = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
            if (_options.Schemas.Any() || _options.Tables.Any())
            {
                sql += " WHERE ";
            }
            if (_options.Schemas.Any())
            {
                sql += $"TABLE_SCHEMA {(_options.Exclude ? "NOT" : "")} IN (";
            }
            foreach (var schema in _options.Schemas)
            {
                sql += $"'{schema}',";
            }
            if (_options.Schemas.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            if (_options.Tables.Any())
            {
                sql += $"{(_options.Schemas.Any() ? " AND " : "")}TABLE_NAME {(_options.Exclude ? "NOT" : "")} IN (";
            }
            foreach (var table in _options.Tables)
            {
                sql += $"'{table}',";
            }
            if (_options.Tables.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            using (var connection = new SqlConnection(_options.SourceConnectionString))
            {
                return await connection.QueryAsync<(string, string)>(sql);
            }
        }

        private async Task<IEnumerable<string>> GetSourceColumnsAsync(string schema, string table)
        {
            var sql = $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = '{schema}' AND TABLE_NAME = '{table}'
                AND COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 0";
            if (_options.Columns.Any())
            {
                sql += $" AND COLUMN_NAME {(_options.Exclude ? "NOT" : "")} IN (";
            }
            foreach (var column in _options.Columns)
            {
                sql += $"'{column}',";
            }
            if (_options.Columns.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            if (_options.ExcludedTypes.Any())
            {
                sql += " AND DATA_TYPE NOT IN (";
            }
            foreach (var typeToExclude in _options.ExcludedTypes)
            {
                sql += $"'{typeToExclude}',";
            }
            if (_options.ExcludedTypes.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            sql += " ORDER BY ORDINAL_POSITION";
            using (var connection = new SqlConnection(_options.SourceConnectionString))
            {
                return await connection.QueryAsync<string>(sql);
            }
        }
    }
}