using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using ClassLibrary1;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace XUnitTestProject1.Infrastructure.Fixtures
{
    public abstract class IntegrationFixtureBase : IDisposable
    {
        protected IConfiguration Configuration => Server.Host.Services.GetService<IConfiguration>();
        public TestServer Server { get; set; }
        protected static ILogger Logger { get; set; }

        protected IntegrationFixtureBase()
        {
            //var hostBuilder = new WebHostBuilder()
            //    .UseStartup<Startup>();

            var hostBuilder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();

            // Build host
            Server = new TestServer(hostBuilder);
        }

        public void SetLogger<T>(ILogger<T> logger)
        {
            Logger = logger;
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

        protected void DropAndCreateDatabase<T>(string connectionString) where T : DbContext
        {
            DropAndCreateDatabase<T>(connectionString, _ => { });
        }

        protected void DropAndCreateDatabase<T>(string connectionString, Action<T> seeder) where T : DbContext
        {
            using (var context = CreateDbContext<T>(connectionString))
            {
                context.Database.EnsureDeleted();
                context.Database.Migrate();
                seeder(context);
            }
        }

        private static T CreateDbContext<T>(string connectionString) where T : DbContext
        {
            var options = new DbContextOptionsBuilder<T>()
                .UseSqlServer(connectionString)
                .Options;
            return (T)Activator.CreateInstance(typeof(T), options);
        }
    }
}