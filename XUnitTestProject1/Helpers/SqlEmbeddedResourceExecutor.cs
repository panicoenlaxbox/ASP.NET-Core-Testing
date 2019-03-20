using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace XUnitTestProject1.Helpers
{
    public class SqlEmbeddedResourceExecutor
    {
        public static async Task ExecuteAsync(string connectionString, Assembly assembly, IEnumerable<string> resources, ITestOutputHelper logger)
        {
            foreach (var resource in resources.OrderBy(r => r))
            {
                await ExecuteAsync(connectionString, assembly, resource, logger);
            }
        }

        public static async Task ExecuteAsync(string connectionString, Assembly assembly, string resource, ITestOutputHelper logger)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            var stopwatch = Stopwatch.StartNew();

            IEnumerable<ParsedStatementSet> statementSets;
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"{resource} not found", nameof(resource));
                }

                using (var streamReader = new StreamReader(stream))
                {
                    var stopwatch3 = Stopwatch.StartNew();
                    statementSets = SqlBatchParser.ParseScriptData(streamReader, batchSize: 50);
                    stopwatch3.Stop();
                    logger.WriteLine($"ParseScripData {stopwatch3.Elapsed:g}");
                }
            }

            foreach (var statementSet in statementSets)
            {
                if (statementSet is ParsedInsertStatementSet insertStatementSet)
                {
                    var stopwatch2 = Stopwatch.StartNew();

                    const int numberOfTasks = 8;
                    var tasks = new List<Task>(); ;

                    var currentTaskIndex = 0;

                    foreach (var statement in insertStatementSet.Inserts)
                    {
                        var task = Task.Run(async () =>
                        {
                            var commandText = $"{insertStatementSet.IdentityInsertOn?.Value}\n{statement.Value}";
                            await ExecuteCommandAsync(connectionString, commandText);
                        });
                        tasks.Add(task);

                        currentTaskIndex++;

                        if (currentTaskIndex == numberOfTasks)
                        {
                            Task.WaitAll(tasks.ToArray());
                            //tasks.ForEach(t => t?.Dispose());
                            tasks.Clear();
                            currentTaskIndex = 0;
                        }
                    }

                    Task.WaitAll(tasks.ToArray());
                    //tasks.ForEach(t => t?.Dispose());

                    stopwatch2.Stop();
                    logger.WriteLine($"{insertStatementSet.Table} {stopwatch2.Elapsed:g}");
                }
                else
                {
                    foreach (var statement in statementSet.Statements)
                    {
                        await ExecuteCommandAsync(connectionString, statement.Value);
                    }
                }
            }

            stopwatch.Stop();
            logger.WriteLine($"Total {stopwatch.Elapsed:g}");
        }

        private static async Task ExecuteCommandAsync(string connectionString, string commandText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}