using System;
using System.Threading.Tasks;
using ClassLibrary1;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace XUnitTestProject1
{
    public class HostFixture : IDisposable
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();

        public HostFixture()
        {
            var hostBuilder = new WebHostBuilder()
                .UseStartup<XUnitTestProject1StartUp>();

            Server = new TestServer(hostBuilder);

            Server.Host.MigrateDbContext<FooContext>(context =>
            {
                context.Foo.Add(new Foo() { Bar = "Bar" });
                context.SaveChanges();
            });

            Checkpoint.TablesToIgnore = new[]
            {
                "__EFMigrationsHistory"
            };
        }

        public static async Task ResetDatabaseAsync()
        {
            await Checkpoint.Reset(@"Server=(LocalDB)\MSSQLLocalDB;Database=Foo;Trusted_Connection=True;");
        }

        public TestServer Server { get; set; }

        public void Dispose()
        {
            Server?.Dispose();
        }

        // DbContext is not thread-safe
        private async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (var scope = Server.Host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public Task ExecuteDbContextAsync(Func<FooContext, Task> action)
        {
            return ExecuteScopeAsync(serviceProvider => 
                action(serviceProvider.GetService<FooContext>()));
        }
    }
}