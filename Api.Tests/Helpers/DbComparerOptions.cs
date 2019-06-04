using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Tests.Helpers
{
    /// <summary>
    /// Comparision options
    /// </summary>
    public class DbComparerOptions
    {
        /// <summary>
        /// Comparision options
        /// </summary>
        /// <param name="sourceConnectionString">Source connection string</param>
        /// <param name="targetConnectionString">Target connection string</param>
        /// <param name="schemas">Schemas to compare, all by default</param>
        /// <param name="tables">Tables to compare. If empty and <param name="exclude">exclude</param> it's true, tables will be __EFMigrationsHistory, SchemaVersions and sysdiagrams</param>
        /// <param name="columns">Columns to compare, all by default. Identity columns will be excluded always</param>
        /// <param name="exclude">It's related to which schemas, tables and columns are used.
        /// If schemas has any value, if exclude is true all schemas will be used except those specified, otherwise, if exclude is false, only those schemas will be used
        /// Regarding to tables, if exclude is true all tables will be used except those specified, otherwise, if exclude is false, only those tables will be used
        /// Regarding with columns, if exclude is true all columns will be used except those specified, otherwise, if exclude is false, only those columns will be used</param>
        /// <param name="count"></param>
        /// <param name="excludedTypes">Column types that will not be compared regardless of the value of <param name="columns">columns</param></param>
        public DbComparerOptions(string sourceConnectionString, string targetConnectionString, IEnumerable<string> schemas = null, IEnumerable<string> tables = null, IEnumerable<string> columns = null, bool exclude = true, bool count = false, IEnumerable<string> excludedTypes = null)
        {
            SourceConnectionString = sourceConnectionString ?? throw new ArgumentNullException(nameof(sourceConnectionString));
            TargetConnectionString = targetConnectionString ?? throw new ArgumentNullException(nameof(targetConnectionString));
            Schemas = schemas ?? Enumerable.Empty<string>();
            if (exclude)
            {
                Tables = new[] { "__EFMigrationsHistory", "SchemaVersions", "sysdiagrams" }.Union(tables ?? Enumerable.Empty<string>());
            }
            else
            {
                Tables = tables ?? throw new ArgumentNullException(nameof(tables));
            }
            Columns = columns ?? Enumerable.Empty<string>();
            // https://docs.microsoft.com/en-us/sql/t-sql/functions/checksum-transact-sql?view=sql-server-2017#arguments
            Exclude = exclude;
            Count = count;
            ExcludedTypes = excludedTypes ?? new[]
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

        public string SourceConnectionString { get; }
        public string TargetConnectionString { get; }
        public IEnumerable<string> Schemas { get; }
        public IEnumerable<string> Tables { get; }
        public IEnumerable<string> Columns { get; }
        public bool Exclude { get; }
        public bool Count { get; }
        public IEnumerable<string> ExcludedTypes { get; }
    }
}