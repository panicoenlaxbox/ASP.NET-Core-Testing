using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

// ReSharper disable PossibleMultipleEnumeration

namespace XUnitTestProject1.Helpers
{
    public class DbComparer
    {
        private readonly string _sourceConnectionString;
        private readonly string _targetConnectionString;
        private readonly IEnumerable<string> _schemas;
        private readonly IEnumerable<string> _tables;
        private readonly IEnumerable<string> _fields;
        private readonly bool _exclude;
        private readonly bool _count;
        private readonly IList<string> _typesToExclude;

        public DbComparer(
            string sourceConnectionString,
            string targetConnectionString,
            IEnumerable<string> schemas,
            IEnumerable<string> tables,
            IEnumerable<string> fields,
            bool exclude,
            bool count)
        {
            _sourceConnectionString = sourceConnectionString ?? throw new ArgumentNullException(nameof(sourceConnectionString));
            _targetConnectionString = targetConnectionString ?? throw new ArgumentNullException(nameof(targetConnectionString));
            _schemas = schemas ?? Enumerable.Empty<string>();
            if (exclude)
            {
                _tables = new[] { "__EFMigrationsHistory", "sysdiagrams" }.Union(tables ?? Enumerable.Empty<string>());
            }
            else
            {
                _tables = tables ?? throw new ArgumentNullException(nameof(tables));
            }
            _fields = fields ?? Enumerable.Empty<string>();
            // https://docs.microsoft.com/en-us/sql/t-sql/functions/checksum-transact-sql?view=sql-server-2017#arguments
            _exclude = exclude;
            _count = count;
            _typesToExclude = new List<string>
            {
                "cursor",
                "image",
                "ntext",
                "text",
                "XML",
                "date",
                "datetime",
                "datetime2",
                "datetimeoffset",
                "time",
                "timestamp"
            };
        }

        public DbComparerResult Compare()
        {
           var result = new DbComparerResult();
            var tables = GetTables(
                _sourceConnectionString,
                _schemas,
                _tables,
                _exclude);
            foreach (var (schema, table) in tables)
            {
                var entry = CreateEntry(
                    _sourceConnectionString,
                    _targetConnectionString,
                    schema,
                    table,
                    _fields,
                    _typesToExclude,
                    _exclude);
                result.Entries.Add(entry);
            }

            if (_count)
            {
                foreach (var entry in result.Entries.Where(e => !e.Match))
                {
                    entry.SourceCount = GetCount(_sourceConnectionString, entry.Schema, entry.Table);
                    entry.TargetCount = GetCount(_targetConnectionString, entry.Schema, entry.Table);
                }
            }

            return result;
        }

        private static int GetCount(string connectionString, string schema, string table)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.ExecuteScalar<int>(
                    $"SELECT COUNT(*) FROM [{schema}].[{table}]");
            }
        }

        private static DbComparerEntryResult CreateEntry(
            string sourceConnectionString,
            string targetConnectionString,
            string schema,
            string table,
            IEnumerable<string> fieldsToExclude,
            IEnumerable<string> typesToExclude,
            bool exclude)
        {
            // https://stackoverflow.com/questions/1560306/calculate-hash-or-checksum-for-a-table-in-sql-server
            // https://stackoverflow.com/questions/11994430/what-conditions-cause-checksum-agg-to-return-0
            
            var sql = "SELECT SUM(CAST(CHECKSUM(";
            foreach (var column in GetColumns(
                sourceConnectionString,
                schema,
                table,
                fieldsToExclude,
                typesToExclude,
                exclude))
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
            using (var connection = new SqlConnection(sourceConnectionString))
            {
                entry.SourceChecksum = connection.ExecuteScalar<long>(sql);
            }
            using (var connection = new SqlConnection(targetConnectionString))
            {
                entry.TargetChecksum = connection.ExecuteScalar<long>(sql);
            }
            return entry;
        }

        private static IEnumerable<(string, string)> GetTables(string connectionString, IEnumerable<string> schemas, IEnumerable<string> tables, bool exclude)
        {
            var sql = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
            if (schemas.Any() || tables.Any())
            {
                sql += " WHERE ";
            }
            if (schemas.Any())
            {
                sql += $"TABLE_SCHEMA {(exclude ? "NOT" : "")} IN (";
            }
            foreach (var schema in schemas)
            {
                sql += $"'{schema}',";
            }
            if (schemas.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            if (tables.Any())
            {
                sql += $"{(schemas.Any() ? " AND " : "")}TABLE_NAME {(exclude ? "NOT" : "")} IN (";
            }
            foreach (var table in tables)
            {
                sql += $"'{table}',";
            }
            if (tables.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<(string, string)>(sql).ToList();
            }
        }

        private static IEnumerable<string> GetColumns(string connectionString, string schema, string table, IEnumerable<string> fields, IEnumerable<string> typesToExclude, bool exclude)
        {
            var sql = $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = '{schema}' AND TABLE_NAME = '{table}'
                AND COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 0";
            if (fields.Any())
            {
                sql += $" AND COLUMN_NAME {(exclude ? "NOT" : "")} IN (";
            }
            foreach (var field in fields)
            {
                sql += $"'{field}',";
            }
            if (fields.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            if (typesToExclude.Any())
            {
                sql += " AND DATA_TYPE NOT IN (";
            }
            foreach (var typeToExclude in typesToExclude)
            {
                sql += $"'{typeToExclude}',";
            }
            if (typesToExclude.Any())
            {
                sql = sql.TrimEnd(',') + ")";
            }
            sql += " ORDER BY ORDINAL_POSITION";
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<string>(sql).ToList();
            }
        }
    }
}