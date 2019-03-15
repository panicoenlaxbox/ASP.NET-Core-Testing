using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary1;
using ClassLibrary1.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTestProject1.Tests
{
    [Collection("HostCollectionFixture")]
    public class CustomersControllerShould
    {
        private readonly HostFixture _fixture;
        private readonly ITestOutputHelper _logger;

        public CustomersControllerShould(HostFixture fixture, ITestOutputHelper logger)
        {
            _fixture = fixture;
            _logger = logger;
        }

        [Fact]
        [ResetDatabase]
        public async Task get_all_works()
        {
            //var httpClient = _fixture.Server.CreateClient();
            //var response2 = await httpClient.GetAsync("api/customers");
            //var response3 = await httpClient.GetAsync(Api.Get.Customer());

            var response = await _fixture.Server
                .CreateHttpApiRequest<CustomersController>(controller => controller.Get())
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim value"),
                }).GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        [ResetDatabase]
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
            //var response = await httpClient.GetAsync(Api.Get.Customer(1));
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

        [Fact]
        [ResetDatabase(executeBefore: true, executeAfter: true, count: true)]
        public async Task get_one_with_sql_seeder_works()
        {
            Customer customer = null;

            await _fixture.ExecuteDbContextAsync(async context =>
            {
                customer = await context.Customers.FirstAsync();
                customer.Name = "Customer 11";
                context.SaveChanges();
            });

            var id = customer.Id;
            var response = await _fixture.Server
                .CreateHttpApiRequest<CustomersController>(controller => controller.Get(id))
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim value"),
                }).GetAsync();

            throw new DivideByZeroException();

            response.EnsureSuccessStatusCode();
        }
    }

    // Use it if we don't use Acheve
}
