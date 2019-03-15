using System;
using System.Threading.Tasks;
using ClassLibrary1;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTestProject1
{
    public abstract class HostFixtureBase : IDisposable
    {
        protected IConfiguration Configuration => Server.Host.Services.GetService<IConfiguration>();
        public TestServer Server { get; set; }

        protected HostFixtureBase()
        {
            //var hostBuilder = new WebHostBuilder()
            //    .UseStartup<Startup>();

            var hostBuilder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();

            // Build host
            Server = new TestServer(hostBuilder);
        }

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

        public Task ExecuteDbContextAsync(Func<ShopContext, Task> action)
        {
            return ExecuteScopeAsync(serviceProvider =>
                action(serviceProvider.GetService<ShopContext>()));
        }

        protected (string, string) ParseConnectionStrings()
        {
            var unique = GetType().Name;
            return (
                string.Format(Configuration.GetConnectionString("DefaultConnection"), unique),
                string.Format(Configuration.GetConnectionString("ConnectionAfter"), $"{unique}_after"));
        }

        protected void CreateDatabase(string connectionString)
        {
            var options = new DbContextOptionsBuilder<ShopContext>()
                .UseSqlServer(connectionString)
                .Options;
            using (var contextAfter = new ShopContext(options))
            {
                contextAfter.Database.Migrate();
            }
        }
    }
}