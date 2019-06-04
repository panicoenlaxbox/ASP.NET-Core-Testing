using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Api.Tests.Helpers
{
    public class SqlEmbeddedResourceExecutor
    {
        private readonly ILogger _logger;

        public SqlEmbeddedResourceExecutor()
        {
            _logger = NullLogger.Instance;
        }

        public SqlEmbeddedResourceExecutor(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(string connectionString, Assembly assembly, IEnumerable<string> resources)
        {
            foreach (var resource in resources)
            {
                await ExecuteAsync(connectionString, assembly, resource);
            }
        }

        public async Task ExecuteAsync(string connectionString, Assembly assembly, string resource, int batchSize = 50)
        {
            _logger.LogDebug($"Starting {nameof(ExecuteAsync)}");

            var stopwatch = Stopwatch.StartNew();

            var statementSets = GetStatementSets(assembly, resource, batchSize);

            await ExecuteStatementSetsAsync(connectionString, statementSets);

            stopwatch.Stop();

            _logger.LogDebug($"Ending {nameof(ExecuteAsync)} {stopwatch.Elapsed:g}");
        }

        private async Task ExecuteStatementSetsAsync(string connectionString, IEnumerable<ParsedStatementSet> statementSets)
        {
            var stopwatch = Stopwatch.StartNew();

            foreach (var statementSet in statementSets)
            {
                if (statementSet is ParsedInsertStatementSet insertStatementSet)
                {
                    var internalStopwatch = Stopwatch.StartNew();

                    const int numberOfTasks = 8;
                    var tasks = new List<Task>();
                    ;

                    var currentTaskIndex = 0;

                    _logger.LogDebug($"{insertStatementSet.Table} {insertStatementSet.Count}");

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

                    internalStopwatch.Stop();
                    _logger.LogDebug($"{insertStatementSet.Table} {internalStopwatch.Elapsed:g}");
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

            _logger.LogDebug($"Ending {nameof(ExecuteStatementSetsAsync)} {stopwatch.Elapsed:g}");
        }

        private IEnumerable<ParsedStatementSet> GetStatementSets(Assembly assembly, string resource, int batchSize)
        {
            var stopwatch = Stopwatch.StartNew();

            var statementSets = Enumerable.Empty<ParsedStatementSet>();

            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"{resource} not found", nameof(resource));
                }

                using (var streamReader = new StreamReader(stream))
                {
                    var internalStopwatch = Stopwatch.StartNew();

                    if (Path.GetExtension(resource).Equals(".zip", StringComparison.CurrentCultureIgnoreCase))
                    {
                        using (var zip = new ZipArchive(streamReader.BaseStream, ZipArchiveMode.Read))
                        {
                            foreach (var entry in zip.Entries.OrderBy(zae => zae.FullName))
                            {
                                using (var zipStream = entry.Open())
                                using (var zipStreamReader = new StreamReader(zipStream))
                                {
                                    statementSets = SqlBatchParser.ParseScriptData(zipStreamReader, batchSize: batchSize);
                                }
                            }
                        }
                    }
                    else
                    {
                        statementSets = SqlBatchParser.ParseScriptData(streamReader, batchSize: batchSize);
                    }

                    internalStopwatch.Stop();

                    _logger.LogDebug($"ParseScripData {resource} {internalStopwatch.Elapsed:g}");
                }
            }

            stopwatch.Stop();

            _logger.LogDebug($"Ending {nameof(GetStatementSets)} {stopwatch.Elapsed:g}");

            return statementSets;
        }

        public async Task ExecuteAsync(string connectionString, Assembly assembly, string type, string methodName, string suffix)
        {
            var finder = new SqlEmbeddedResourceFinder();
            var resources = finder.Find(assembly, type, methodName, new[] { suffix });
            if (!resources[suffix].Any())
            {
                throw new Exception($"There are no '{suffix}' resources to execute");
            }
            await ExecuteAsync(connectionString, assembly, resources[suffix]);
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