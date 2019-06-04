using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Tests.Infrastructure.Fixtures
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

                #region SqlMigrator
                //var builder = new SqlMigratorOptionsBuilder();
                //builder.WithLogger(Server.Host.Services.GetService<ILoggerFactory>()
                //    .CreateLogger<SqlMigrator.SqlMigrator>());
                //builder.WithConnectionString(connectionString);
                //var folder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //builder.WithExecutionFolder(folder);

                //var migrator = new SqlMigrator.SqlMigrator(builder.Build());
                //migrator.Run();
                #endregion

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