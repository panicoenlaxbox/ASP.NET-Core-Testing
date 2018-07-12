using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary1;
using ClassLibrary1.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace XUnitTestProject1
{
    [Collection("HostCollectionFixture")]
    public class ValuesControllerShould
    {
        private readonly HostFixture _fixture;

        public ValuesControllerShould(HostFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        [ResetDatabase]
        public async Task get_all_successfully()
        {
            //var httpClient = _fixture.Server.CreateClient();
            //var response = await httpClient.GetAsync("api/values");
            var response = await _fixture.Server
                .CreateHttpApiRequest<ValuesController>(controller => controller.Get())
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim is very important"),
                }).GetAsync();
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        [ResetDatabase]
        public async Task get_foo_successfully()
        {
            var foo = new Foo()
            {
                Bar = "Bar"
            };
            await _fixture.ExecuteDbContextAsync(async context =>
            {
                await context.Foo.AddAsync(foo);
                await context.SaveChangesAsync();
            });

            //var httpClient = _fixture.Server.CreateClient();
            //var response = await httpClient.GetAsync(Api.Get.Foo(1));
            var id = foo.Id;
            var response = await _fixture.Server
                .CreateHttpApiRequest<ValuesController>(controller => controller.Get(id))
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim is very important"),
                }).GetAsync();

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        [ResetDatabase]
        public async Task get_all_successfully_2()
        {
            var requestBuilder = _fixture.Server
                .CreateHttpApiRequest<ValuesController>(controller => controller.Get())
                .WithIdentity(new[]
                {
                    new Claim("myclaim", "myclaim is very important"),
                });
            var response = await requestBuilder.GetAsync();
            response.EnsureSuccessStatusCode();
        }
    }

    public static class Api
    {
        public static class Get
        {
            public static string Foo(int id)
            {
                return $"api/values/{id}";
            }
        }
    }
}
