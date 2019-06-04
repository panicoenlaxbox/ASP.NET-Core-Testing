using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Respawn;

namespace Api.Tests.Infrastructure.Fixtures
{
    public class CountriesFixture : IntegrationFixtureBase
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();
        public static string ConnectionString { get; set; }

        public CountriesFixture()
        {
            ConnectionString = string.Format(Configuration.GetConnectionString("DefaultConnection"), $"{GetType().Name}");
            Configuration["ConnectionStrings:DefaultConnection"] = ConnectionString;

            DropAndCreateDatabase<ShopContext>(ConnectionString, context =>
            {
                // This tables have to be excluded in Checkpoint.TablesToIgnore if we want to have them in every test
                // Furthermore, we could have data seeding in ef configurations with the HasData method
            });

            Checkpoint.TablesToIgnore = Constants.TablesToIgnore;
        }

        // ReSharper disable once UnusedMember.Global
        public static async Task ResetDatabaseAsync()
        {
            await Checkpoint.Reset(ConnectionString);
        }
    }
}