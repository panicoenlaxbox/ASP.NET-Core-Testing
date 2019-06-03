using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;
using XUnitTestProject1.Helpers;
using XUnitTestProject1.Infrastructure.Loggers;

namespace XUnitTestProject1.Infrastructure
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

        protected async Task CompareAsync(string actualConnectionString, string expectedConnectionString)
        {
            await CompareAsync(actualConnectionString, expectedConnectionString, NullLogger.Instance);
        }

        protected async Task CompareAsync(string actualConnectionString, string expectedConnectionString, ILogger logger)
        {
            logger.LogDebug("Starting comparision");
            var stopwatch = Stopwatch.StartNew();

            var dbComparer = new DbComparer(
                actualConnectionString,
                expectedConnectionString,
                count: true);
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