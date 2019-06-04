using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Microsoft.Extensions.Configuration;
using Respawn;

namespace Api.Tests.Infrastructure.Fixtures
{
    public class CustomersFixture : IntegrationFixtureBase
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();
        public static string ConnectionString { get; set; }

        public CustomersFixture()
        {
            ConnectionString = string.Format(Configuration.GetConnectionString("DefaultConnection"), $"{GetType().Name}");
            Configuration["ConnectionStrings:DefaultConnection"] = ConnectionString;

            DropAndCreateDatabase<ShopContext>(ConnectionString, context =>
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

            Checkpoint.TablesToIgnore = Constants.TablesToIgnore.Concat(
                new[] { "Countries" }).ToArray();
        }

        // ReSharper disable once UnusedMember.Global
        public static async Task ResetDatabaseAsync()
        {
            await Checkpoint.Reset(ConnectionString);
        }
    }
}