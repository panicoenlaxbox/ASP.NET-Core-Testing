using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostExtensions
    {
        // https://gist.github.com/unaizorrilla/17ee153933ef253a22b6a7f6a744e423

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext> seeder) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();

                var context = serviceProvider.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    context.Database.Migrate();

                    seeder(context);

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}.");
                }
            }

            return webHost;
        }
    }
}