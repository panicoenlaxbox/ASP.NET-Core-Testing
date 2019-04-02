using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using XUnitTestProject1.Infrastructure.Fixtures;

namespace XUnitTestProject1
{
    [Collection("OmpCollection")]
    public class OmpControllerShould : DatabaseTestCaseBase
    {
        private readonly OmpFixture _fixture;
        private ILogger<OmpControllerShould> _logger;

        public OmpControllerShould(OmpFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            SetLogger(output);
        }

        private void SetLogger(ITestOutputHelper output)
        {
            _logger = CreateLogger<OmpControllerShould>(_fixture.Server.Host.Services, output);
            _fixture.SetLogger(_logger);
        }

        [Fact(Skip = "faltan ficheros .sql")]
        [ResetDatabase(nameof(OmpFixture))]
        public async Task example()
        {
            // Arrange
            await ExecuteAsync(OmpFixture.ConnectionString, nameof(example), false, _logger);

            // Act            

            // Assert
            await ExecuteAsync(OmpFixture.ConnectionStringAfter, nameof(example), true, _logger);
            await CompareAsync(OmpFixture.ConnectionString, OmpFixture.ConnectionStringAfter, _logger);
        }
    }
}