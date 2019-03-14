using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace XUnitTestProject1
{
    public class DbComparer
    {
        private readonly string _sourceConnectionString;
        private readonly string _targetConnectionString;
        private readonly IEnumerable<string> _tablesToExclude;
        private readonly IEnumerable<string> _fieldsToExclude;
        private readonly string[] _typesToExclude = { "cursor", "image", "ntext", "text", "XML" };

        public DbComparer(string sourceConnectionString, string targetConnectionString, IEnumerable<string> tablesToExclude,
            IEnumerable<string> fieldsToExclude)
        {
            _sourceConnectionString = sourceConnectionString ?? throw new ArgumentNullException(nameof(sourceConnectionString));
            _targetConnectionString = targetConnectionString ?? throw new ArgumentNullException(nameof(targetConnectionString));
            _tablesToExclude = new[] { "__EFMigrationsHistory" }.Union(tablesToExclude ?? Enumerable.Empty<string>());
            _fieldsToExclude = new[] { "Id" }.Union(fieldsToExclude ?? Enumerable.Empty<string>());
        }

        public DbComparerResult Compare()
        {
            var result = new DbComparerResult();

            foreach (var (schema, table) in GetTables())
            {
                var sql = "SELECT CHECKSUM(";
                foreach (var column in GetColumns(table))
                {
                    sql += $"[{column}],";
                }
                sql = sql.TrimEnd(',') +
                          $@") FROM [{schema}].[{table}]";
                var entry = new DbComparerEntryResult();
                using (var connection = new SqlConnection(_sourceConnectionString))
                {
                    var checksum = connection.ExecuteScalar<int>(sql);
                    entry.TableName = table;
                    entry.SourceChecksum = checksum;
                }
                using (var connection = new SqlConnection(_targetConnectionString))
                {
                    var checksum = connection.ExecuteScalar<int>(sql);
                    entry.TargetChecksum = checksum;
                }
                result.Entries.Add(entry);
            }

            return result;
        }

        private IEnumerable<(string, string)> GetTables()
        {
            var sql = @"SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
                      WHERE TABLE_NAME NOT IN (";
            foreach (var tableToExclude in _tablesToExclude)
            {
                sql += $"'{tableToExclude}',";
            }

            sql = sql.TrimEnd(',') + ")";
            using (var connection = new SqlConnection(_sourceConnectionString))
            {
                return connection.Query<(string, string)>(sql).ToList();
            }
        }

        private IEnumerable<string> GetColumns(string tableName)
        {
            var sql = $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = '{tableName}'";
            if (_fieldsToExclude.Any())
            {
                sql += " AND COLUMN_NAME NOT IN (";
            }
            foreach (var fieldToExclude in _fieldsToExclude)
            {
                sql += $"'{fieldToExclude}',";
            }
            if (_fieldsToExclude.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            sql += " AND DATA_TYPE NOT IN (";
            foreach (var typeToExclude in _typesToExclude)
            {
                sql += $"'{typeToExclude}',";
            }
            sql = sql.TrimEnd(',') + ")";
            sql += " ORDER BY ORDINAL_POSITION";
            using (var connection = new SqlConnection(_sourceConnectionString))
            {
                return connection.Query<string>(sql).ToList();
            }
        }
    }
}