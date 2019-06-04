using System.Security.Claims;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Entities;
using Api.Tests.Infrastructure.Extensions;
using Api.Tests.Infrastructure.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Api.Tests
{
    [Collection("CountriesCollection")]

    public class CountriesControllerShould
    {
        private readonly CountriesFixture _fixture;

        public CountriesControllerShould(CountriesFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [ResetDatabase(fixture: nameof(CountriesFixture))]
        public async Task get_one()
        {
            Country country = null;

            await _fixture.ExecuteDbContextAsync(async context =>
            {
                country = new Country() { Name = "Ireland" };
                context.Countries.Add(country);
                await context.SaveChangesAsync();
            });

            var id = country.Id;
            var response = await _fixture.Server
                .CreateHttpApiRequest<CountriesController>(controller => controller.Get(id))
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim value"),
                }).GetAsync();

            response.EnsureSuccessStatusCode();

            (await response.GetAsyncTo<Country>()).Should().BeEquivalentTo(
                new Country() { Id = id, Name = "Ireland" });
        }
    }
}