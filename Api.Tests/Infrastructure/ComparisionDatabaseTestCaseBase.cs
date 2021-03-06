using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Api.Tests.Helpers;
using Api.Tests.Infrastructure.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;

namespace Api.Tests.Infrastructure
{
    public abstract class ComparisionDatabaseTestCaseBase
    {
        protected async Task ExecuteAsync(string connectionString, string methodName, bool expected)
        {
            await ExecuteAsync(connectionString, methodName, expected, NullLogger.Instance);
        }

        protected async Task ExecuteAsync(string connectionString, string methodName, bool expected, ILogger logger)
        {
            var executor = new SqlEmbeddedResourceExecutor(logger);
            await executor.ExecuteAsync(connectionString, Assembly.GetExecutingAssembly(), GetType().Name, methodName, expected ? "expected" : "actual");
        }

        protected async Task CompareAsync(string sourceConnectionString, string targetConnectionString)
        {
            await CompareAsync(sourceConnectionString, targetConnectionString, NullLogger.Instance);
        }

        protected async Task CompareAsync(string sourceConnectionString, string targetConnectionString, ILogger logger)
        {
            await CompareAsync(new DbComparerOptions(sourceConnectionString, targetConnectionString), logger);
        }

        protected async Task CompareAsync(DbComparerOptions options, ILogger logger)
        {
            logger.LogDebug("Starting comparision");
            var stopwatch = Stopwatch.StartNew();

            var dbComparer = new DbComparer(options);
            var comparerResult = await dbComparer.CompareAsync();
            if (!comparerResult)
            {
                stopwatch.Stop();
                logger.LogDebug($"Ending comparision {stopwatch.Elapsed:g}");
                throw new Exception(comparerResult.Prettify());
            }

            stopwatch.Stop();
            logger.LogDebug($"Ending comparision {stopwatch.Elapsed:g}");
        }

        protected ILogger<T> CreateLogger<T>(IServiceProvider serviceProvider, ITestOutputHelper output)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new XUnitLoggerProvider(output));
            return loggerFactory.CreateLogger<T>();
        }
    }
}