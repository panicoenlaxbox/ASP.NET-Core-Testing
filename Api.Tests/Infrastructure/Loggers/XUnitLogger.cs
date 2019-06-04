using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Api.Tests.Infrastructure.Loggers
{
    public class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _categoryName;

        public XUnitLogger(ITestOutputHelper testOutputHelper, string categoryName)
        {
            _testOutputHelper = testOutputHelper;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) => NoopDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                var message = $"{_categoryName} [{eventId}]\n{formatter(state, exception)}";
                _testOutputHelper.WriteLine(message);
                if (exception != null)
                    _testOutputHelper.WriteLine(exception.ToString());
            }
            catch (InvalidOperationException)
            {
                // BeforeAfterTestAttribute has an active test, but ICollectionFixture<> does not have

                // System.InvalidOperationException: 'There is no currently active test.'
            }
        }

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {

            }
        }
    }
}
