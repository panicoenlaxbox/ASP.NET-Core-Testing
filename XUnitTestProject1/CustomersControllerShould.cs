using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary1;
using ClassLibrary1.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Xunit.Abstractions;
using XUnitTestProject1.Helpers;
using XUnitTestProject1.Infrastructure.Fixtures;
using XUnitTestProject1.Infrastructure.TestData;

namespace XUnitTestProject1
{
    [Collection("CustomersCollection")]
    public class CustomersControllerShould
    {
        private readonly CustomersFixture _fixture;
        private readonly ITestOutputHelper _logger;

        public CustomersControllerShould(CustomersFixture fixture, ITestOutputHelper logger)
        {
            _fixture = fixture;
            _logger = logger;
        }

        [Fact]
        [EnhancedResetDatabase(fixture: nameof(CustomersFixture))]
        public async Task get_all_works()
        {
            //var httpClient = _fixture.Server.CreateClient();
            //var response2 = await httpClient.GetAsync("api/customers");
            //var response3 = await httpClient.GetAsync(ApiRoutes.Get.Customer());

            var response = await _fixture.Server
                .CreateHttpApiRequest<CustomersController>(controller => controller.Get())
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim value"),
                }).GetAsync();

            response.EnsureSuccessStatusCode();
            (await response.GetTo<IEnumerable<Customer>>()).Count().Should().Be(0);
        }

        [Fact]
        [EnhancedResetDatabase(fixture: nameof(CustomersFixture))]
        public async Task get_one_works()
        {
            // Arrange/Setup/Given

            //var customer = (Customer)Build.Customer().WithName("Customer 1");

            //var customer = CustomerObjectMother.CustomerWithName();

            var customer = Given.A.CustomerWithName();

            //var customer = new Customer()
            //{
            //    Name = "Customer 1"
            //};
            await _fixture.ExecuteDbContextAsync(async context =>
            {
                await context.Customers.AddAsync(customer);
                await context.SaveChangesAsync();
            });

            //var httpClient = _fixture.Server.CreateClient();
            //var response = await httpClient.GetAsync(ApiRoutes.Get.Customer(1));
            var id = customer.Id;

            // Act/Exercise/When

            var response = await _fixture.Server
                .CreateHttpApiRequest<CustomersController>(controller => controller.Get(id))
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim value"),
                }).GetAsync();


            // Assert/Verify/Then

            response.EnsureSuccessStatusCode();

            (await response.GetTo<Customer>()).Name.Should().Be("Customer 1");
        }
    }
}
