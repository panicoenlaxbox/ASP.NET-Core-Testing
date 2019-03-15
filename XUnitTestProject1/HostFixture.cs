using System;
using System.Threading.Tasks;
using ClassLibrary1;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace XUnitTestProject1
{
    public class HostFixture : IDisposable
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();
        private static readonly Checkpoint CheckpointAfter = new Checkpoint();

        public static string ConnectionString { get; set; }
        public static string ConnectionStringAfter { get; set; }

        public HostFixture()
        {
            //var hostBuilder = new WebHostBuilder()
            //    .UseStartup<Startup>();

            var hostBuilder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();

            // Build host
            Server = new TestServer(hostBuilder);

            // We must read configuration after host has been built
            const string unique = nameof(HostFixture);
            ConnectionString = string.Format(Configuration.GetConnectionString("DefaultConnection"), unique);
            ConnectionStringAfter = string.Format(Configuration.GetConnectionString("ConnectionAfter"), $"{unique}_after");

            // Update connection strings
            Configuration["ConnectionStrings:DefaultConnection"] = ConnectionString;
            Configuration["ConnectionStrings:ConnectionStringAfter"] = ConnectionStringAfter;

            Server.Host.MigrateDbContext<ShopContext>(context =>
            {
                // This tables have to be excluded in Checkpoint.TablesToIgnore if we want to have them in every test
                // Furthermore, we could have data seeding in ef configurations with the HasData method

                context.Countries.Add(new Country()
                {
                    Name = "United Kingdom"
                });

                context.SaveChanges();
            });

            var options = new DbContextOptionsBuilder<ShopContext>()
                .UseSqlServer(ConnectionStringAfter)
                .Options;
            using (var contextAfter = new ShopContext(options))
            {
                contextAfter.Database.Migrate();
            }

            Checkpoint.TablesToIgnore = new[]
            {
                "__EFMigrationsHistory",
                "Countries"
            };

            CheckpointAfter.TablesToIgnore = new[]
            {
                "__EFMigrationsHistory"
            };
        }

        private IConfiguration Configuration => Server.Host.Services.GetService<IConfiguration>();

        public static async Task ResetDatabaseAsync(bool after = false)
        {
            var nameOrConnectionString = after ? ConnectionStringAfter : ConnectionString;
            await Checkpoint.Reset(nameOrConnectionString);
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

        public Task ExecuteDbContextAsync(Func<ShopContext, Task> action)
        {
            return ExecuteScopeAsync(serviceProvider =>
                action(serviceProvider.GetService<ShopContext>()));
        }
    }

    public class HostFixture2 : IDisposable
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();
        private static readonly Checkpoint CheckpointAfter = new Checkpoint();

        public static string ConnectionString { get; set; }
        public static string ConnectionStringAfter { get; set; }

        public HostFixture2()
        {
            //var hostBuilder = new WebHostBuilder()
            //    .UseStartup<Startup>();

            var hostBuilder = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();

            // Build host
            Server = new TestServer(hostBuilder);

            // We must read configuration after host has been built
            const string unique = nameof(HostFixture2);
            ConnectionString = string.Format(Configuration.GetConnectionString("DefaultConnection"), unique);
            ConnectionStringAfter = string.Format(Configuration.GetConnectionString("ConnectionAfter"), $"{unique}_after");

            // Update connection strings
            Configuration["ConnectionStrings:DefaultConnection"] = ConnectionString;
            Configuration["ConnectionStrings:ConnectionStringAfter"] = ConnectionStringAfter;

            Server.Host.MigrateDbContext<ShopContext>(context =>
            {
                // This tables have to be excluded in Checkpoint.TablesToIgnore if we want to have them in every test
                // Furthermore, we could have data seeding in ef configurations with the HasData method

                context.Countries.AddRange(new Country[] {
                    new Country()
                    {
                        Name = "Spain"
                    },
                    new Country()
                    {
                        Name = "France"
                    },
                    new Country()
                    {
                        Name = "United Kingdom"
                    }
                });

                context.SaveChanges();
            });

            var options = new DbContextOptionsBuilder<ShopContext>()
                .UseSqlServer(ConnectionStringAfter)
                .Options;
            using (var contextAfter = new ShopContext(options))
            {
                contextAfter.Database.Migrate();
            }

            Checkpoint.TablesToIgnore = new[]
            {
                "__EFMigrationsHistory",
                "Countries"
            };

            CheckpointAfter.TablesToIgnore = new[]
            {
                "__EFMigrationsHistory"
            };
        }

        private IConfiguration Configuration => Server.Host.Services.GetService<IConfiguration>();

        public static async Task ResetDatabaseAsync(bool after = false)
        {
            var nameOrConnectionString = after ? ConnectionStringAfter : ConnectionString;
            await Checkpoint.Reset(nameOrConnectionString);
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

        public Task ExecuteDbContextAsync(Func<ShopContext, Task> action)
        {
            return ExecuteScopeAsync(serviceProvider =>
                action(serviceProvider.GetService<ShopContext>()));
        }
    }
}