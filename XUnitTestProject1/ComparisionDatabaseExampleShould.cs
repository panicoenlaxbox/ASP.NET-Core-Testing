using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using XUnitTestProject1.Infrastructure;
using XUnitTestProject1.Infrastructure.Fixtures;

namespace XUnitTestProject1
{
    [Collection("ComparisionDatabaseExampleCollection")]
    public class ComparisionDatabaseExampleShould : ComparisionDatabaseTestCaseBase
    {
        private readonly ComparisionDatabaseExampleFixture _fixture;
        private ILogger<ComparisionDatabaseExampleShould> _logger;

        public ComparisionDatabaseExampleShould(ComparisionDatabaseExampleFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            SetLogger(output);
        }

        private void SetLogger(ITestOutputHelper output)
        {
            _logger = CreateLogger<ComparisionDatabaseExampleShould>(_fixture.Server.Host.Services, output);
            _fixture.SetLogger(_logger);
        }

        [Fact]
        [ResetDatabase(nameof(ComparisionDatabaseExampleFixture))]
        public async Task simple_example()
        {
            // Arrange
            const string methodName = nameof(simple_example);
            await ExecuteAsync(ComparisionDatabaseExampleFixture.ActualConnectionString, methodName, expected: false, logger: _logger);

            // Act            

            // Assert
            await ExecuteAsync(ComparisionDatabaseExampleFixture.ExpectedConnectionString, methodName, expected: true, logger: _logger);

            await CompareAsync(
                ComparisionDatabaseExampleFixture.ActualConnectionString, 
                ComparisionDatabaseExampleFixture.ExpectedConnectionString, 
                _logger);
        }
    }
}