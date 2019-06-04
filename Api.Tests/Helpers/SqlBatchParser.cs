using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Api.Tests.Helpers
{
    public class ParsedStatementSet
    {
        public IEnumerable<ParsedStatement> Statements { get; }

        public ParsedStatementSet(IEnumerable<ParsedStatement> statements)
        {
            Statements = statements;
        }
    }

    public class ParsedInsertStatementSet : ParsedStatementSet
    {
        public string Table => Statements.First().Table;

        public int Count => Inserts.Sum(stmt => (int)stmt.BatchSize);

        public ParsedStatement IdentityInsertOn
        {
            get
            {
                return Statements.SingleOrDefault(s => s.Kind == ParsedStatementKind.IdentityInsertOn);
            }
        }

        public ParsedStatement IdentityInsertOff
        {
            get
            {
                return Statements.SingleOrDefault(s => s.Kind == ParsedStatementKind.IdentityInsertOff);
            }
        }

        public IEnumerable<ParsedStatement> Inserts
        {
            get { return Statements.Where(s => s.Kind == ParsedStatementKind.Insert); }
        }

        public ParsedInsertStatementSet(IEnumerable<ParsedStatement> statements) : base(statements)
        {
        }
    }

    public class ParsedStatement
    {
        public string Table { get; }
        public string Value { get; }
        public ParsedStatementKind Kind { get; }
        public int? BatchSize { get; set; }

        public ParsedStatement(string table, string value, ParsedStatementKind kind, int? batchSize = null)
        {
            Table = table;
            Value = value;
            Kind = kind;
            BatchSize = batchSize;
        }
    }

    public enum ParsedStatementKind
    {
        IdentityInsertOn,
        IdentityInsertOff,
        Insert,
        NoCheckConstraint,
        CheckConstraint
    }

    public static class StringExtensions
    {
        public static string EnsureEndsWith(this string instance, string value)
        {
            if (!instance.EndsWith(value))
            {
                instance += value;
            }

            return instance;
        }

        public static string RemoveTrailing(this string instance, string value)
        {
            if (instance.EndsWith(value))
            {
                instance = instance.Substring(0, instance.Length - value.Length);
            }

            return instance;
        }
    }

    public class SqlBatchParser
    {
        //public IEnumerable<string> ParseScriptData(string batch, string separator = "GO")
        //{
        //    var pattern = $@"(^\s*{separator}\s*$)";
        //    const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
        //    return Regex.Split(batch, pattern, options)
        //        .Select(s => s.TrimStart('\n', '\r'))
        //        .Select(s => s.TrimEnd('\n', '\r'))
        //        .Select(s => s.Trim())
        //        .Where(p => !p.Equals(separator) && !string.IsNullOrWhiteSpace(p));
        //}

        public static IEnumerable<ParsedStatementSet> ParseScriptData(StreamReader streamReader, string separator = "GO", int batchSize = 100)
        {
            string GetTableFromIdentityInsertStatement(string statement)
            {
                var j = statement.LastIndexOf(" ", StringComparison.CurrentCultureIgnoreCase);
                return statement.Substring("SET IDENTITY_INSERT ".Length, j - "SET IDENTITY_INSERT ".Length);
            }

            string GetTableFromInsertStatement(string statement)
            {
                return statement.Substring("INSERT ".Length,
                    statement.IndexOf(" (", StringComparison.CurrentCultureIgnoreCase) - "INSERT ".Length);
            }

            string NormalizeStatement(string statement)
            {
                return statement.RemoveTrailing(",\n").EnsureEndsWith(";");
            }

            var tables = new List<string>();
            var statements = new List<ParsedStatement>();

            var previousTable = string.Empty;
            var sql = new StringBuilder();
            var reset = false;
            var currentBatchSize = 0;

            var line = streamReader.ReadLine();

            while (line != null)
            {
                line = line.Trim();

                if (line == string.Empty || line.Equals(separator, StringComparison.CurrentCultureIgnoreCase))
                {
                    line = streamReader.ReadLine();
                    continue;
                }

                if (reset)
                {
                    previousTable = string.Empty;
                    sql.Clear();
                    currentBatchSize = 0;

                    reset = false;
                }

                if (line.StartsWith("INSERT [", StringComparison.CurrentCultureIgnoreCase))
                {
                    var currentTable = GetTableFromInsertStatement(line);

                    var i = line.IndexOf(") VALUES (", StringComparison.CurrentCultureIgnoreCase);

                    if (string.IsNullOrWhiteSpace(previousTable))
                    {
                        previousTable = currentTable;
                        if (!tables.Contains(previousTable))
                        {
                            tables.Add(previousTable);
                        }
                        sql.Clear();
                        sql.Append(line.Substring(0, i + ") VALUES".Length) + "\n");
                    }

                    if (!previousTable.Equals(currentTable, StringComparison.CurrentCultureIgnoreCase))
                    {
                        statements.Add(new ParsedStatement(currentTable, NormalizeStatement(sql.ToString()), ParsedStatementKind.Insert, currentBatchSize));

                        sql.Clear();
                        sql.Append(line.Substring(0, i + ") VALUES".Length) + "\n");
                    }

                    sql.Append(line.Substring(i + ") VALUES".Length + 1) + ",\n");

                    currentBatchSize++;

                    if (currentBatchSize == batchSize)
                    {
                        statements.Add(new ParsedStatement(currentTable, NormalizeStatement(sql.ToString()), ParsedStatementKind.Insert, currentBatchSize));

                        reset = true;
                    }
                }
                else
                {
                    if (sql.Length > 0)
                    {
                        statements.Add(new ParsedStatement(previousTable, NormalizeStatement(sql.ToString()), ParsedStatementKind.Insert, currentBatchSize));

                        reset = true;
                    }

                    if (line.StartsWith("SET IDENTITY_INSERT", StringComparison.CurrentCultureIgnoreCase))
                    {
                        statements.Add(
                            new ParsedStatement(
                                GetTableFromIdentityInsertStatement(line),
                                line.EnsureEndsWith(";"),
                                line.EndsWith("ON") ? ParsedStatementKind.IdentityInsertOn : ParsedStatementKind.IdentityInsertOff));
                    }
                }

                line = streamReader.ReadLine();
            }

            if (sql.Length > 0 && !reset)
            {
                statements.Add(new ParsedStatement(previousTable, NormalizeStatement(sql.ToString()), ParsedStatementKind.Insert, currentBatchSize));
            }

            var result = new List<ParsedStatementSet>
            {
                new ParsedStatementSet(tables
                    .Select(s => new ParsedStatement(s, $"ALTER TABLE {s} NOCHECK CONSTRAINT ALL;",
                        ParsedStatementKind.NoCheckConstraint)))
            };

            foreach (var table in tables)
            {
                result.Add(new ParsedInsertStatementSet(
                    statements.Where(s =>
                        s.Table.Equals(table, StringComparison.CurrentCultureIgnoreCase)
                        && new[]
                        {
                            ParsedStatementKind.IdentityInsertOff,
                            ParsedStatementKind.IdentityInsertOn,
                            ParsedStatementKind.Insert
                        }.Contains(s.Kind))));
            }

            result.Add(new ParsedStatementSet(tables.Select(s => new ParsedStatement(s,
                $"ALTER TABLE {s} WITH CHECK CHECK CONSTRAINT ALL;", ParsedStatementKind.CheckConstraint))));

            return result;
        }

        public IEnumerable<ParsedStatementSet> ParseScriptData(string path, string separator = "GO", int batchSize = 100)
        {
            using (var fileStream = File.OpenRead(path))
            using (var streamReader = new StreamReader(fileStream))
            {
                return ParseScriptData(streamReader, separator, batchSize);
            }
        }

    }
}