using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary1;
using ClassLibrary1.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTestProject1.Tests
{
    [Collection("HostCollectionFixture2")]
    public class CustomersController2Should
    {
        public readonly HostFixture2 Fixture;
        private readonly ITestOutputHelper _logger;

        public CustomersController2Should(HostFixture2 fixture, ITestOutputHelper logger)
        {
            Fixture = fixture;
            _logger = logger;
        }

        [Fact]
        [ResetDatabase(collectionFixture: "XUnitTestProject1.HostFixture2", executeBefore: true, executeAfter: true, count: true)]
        public async Task get_one_with_sql_seeder_works()
        {
            Customer customer = null;

            await Fixture.ExecuteDbContextAsync(async context =>
            {
                customer = await context.Customers.FirstAsync();
                //customer.Name = "Customer 11";
                //context.SaveChanges();
            });

            var id = customer.Id;
            var response = await Fixture.Server
                .CreateHttpApiRequest<CustomersController>(controller => controller.Get(id))
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim value"),
                }).GetAsync();

            response.EnsureSuccessStatusCode();
        }
    }
}