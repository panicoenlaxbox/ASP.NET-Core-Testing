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

namespace XUnitTestProject1
{
    #region Builder & ObjectMother 
    public static class Build
    {
        public static CustomerBuilder Customer()
        {
            return new CustomerBuilder();
        }
    }

    public class CustomerBuilder
    {
        private readonly Customer _customer;

        public CustomerBuilder()
        {
            _customer = new Customer();
        }

        public CustomerBuilder WithName(string name)
        {
            _customer.Name = name;
            return this;
        }

        public CustomerBuilder WithId(int id)
        {
            _customer.Id = id;
            return this;
        }

        public static implicit operator Customer(CustomerBuilder builder)
        {
            return builder._customer;
        }
    }

    public static class CustomerObjectMother
    {
        public static Customer CustomerWithName()
        {
            return Build.Customer()
                .WithName("Customer 1");
        }
    }

    #region DSL

    public static class Given
    {
        public static class A
        {
            public static Customer CustomerWithName()
            {
                return CustomerObjectMother.CustomerWithName();
            }
        }
    }

    #endregion



    #endregion


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
        [ResetDatabase(executeBefore: true, executeAfter: true)]
        public async Task get_one_with_sql_seeder_works()
        {
            Customer customer = null;

            await _fixture.ExecuteDbContextAsync(async context =>
            {
                customer = await context.Customers.FirstAsync();
            });

            var id = customer.Id;
            var response = await _fixture.Server
                .CreateHttpApiRequest<CustomersController>(controller => controller.Get(id))
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim value"),
                }).GetAsync();

            response.EnsureSuccessStatusCode();

            (await response.GetTo<Customer>()).Name.Should().Be("Customer 1");

        }
    }

    // Use it if we don't use Acheve
    public static class Api
    {
        public static class Get
        {
            public static string Customer()
            {
                return "api/customers";
            }
            public static string Customer(int id)
            {
                return $"{Customer()}/{id}";
            }
        }
    }
}
