using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary1;
using ClassLibrary1.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;
using XUnitTestProject1.Infrastructure.Fixtures;

namespace XUnitTestProject1
{
    [Collection("CountriesCollection")]
    public class CountriesControllerShould
    {
        public readonly CountriesFixture Fixture;
        private readonly ITestOutputHelper _logger;

        public CountriesControllerShould(CountriesFixture fixture, ITestOutputHelper logger)
        {
            Fixture = fixture;
            _logger = logger;
        }

        [Fact]
        [EnhancedResetDatabase(fixture: nameof(CountriesFixture), executeBefore: true, executeAfter: true, count:true)]
        public async Task get_one_works()
        {
            Country country = null;

            await Fixture.ExecuteDbContextAsync(async context =>
            {
                country = new Country() { Name = "Ireland" };
                context.Countries.Add(country);
                await context.SaveChangesAsync();
            });

            var id = country.Id;
            var response = await Fixture.Server
                .CreateHttpApiRequest<CountriesController>(controller => controller.Get(id))
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim value"),
                }).GetAsync();

            response.EnsureSuccessStatusCode();
        }
    }
}