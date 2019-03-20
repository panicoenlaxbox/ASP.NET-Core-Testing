using System.Threading.Tasks;
using ClassLibrary1;
using Respawn;

namespace XUnitTestProject1.Infrastructure.Fixtures
{
    public class CountriesFixture : IntegrationFixtureBase
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();
        private static readonly Checkpoint CheckpointAfter = new Checkpoint();
        public static string ConnectionString { get; set; }
        public static string ConnectionStringAfter { get; set; }

        public CountriesFixture()
        {
            // We must read configuration after host has been built
            (ConnectionString, ConnectionStringAfter) = ParseConnectionStrings();

            // Update connection strings
            Configuration["ConnectionStrings:DefaultConnection"] = ConnectionString;
            Configuration["ConnectionStrings:ConnectionAfter"] = ConnectionStringAfter;

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

            DropAndCreateDatabase<ShopContext>(ConnectionStringAfter);

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

        // ReSharper disable once UnusedMember.Global
        public static async Task ResetDatabaseAsync(bool after = false)
        {
            var connectionString = after ? ConnectionStringAfter : ConnectionString;
            await Checkpoint.Reset(connectionString);
        }
    }
}