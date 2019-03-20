using System.Threading.Tasks;
using ClassLibrary1;
using Respawn;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XUnitTestProject1.Infrastructure.Fixtures
{
    public class OmpFixture : IntegrationFixtureBase
    {
        private static readonly Checkpoint Checkpoint = new Checkpoint();
        private static readonly Checkpoint CheckpointAfter = new Checkpoint();
        public static string ConnectionString { get; set; }
        public static string ConnectionStringAfter { get; set; }

        public OmpFixture(IMessageSink messageSink)
        {
            var message = new DiagnosticMessage("I will appear in dotnet test");
            messageSink.OnMessage(message);

            // We must read configuration after host has been built
            (ConnectionString, ConnectionStringAfter) = ParseConnectionStrings();

            // Update connection strings
            Configuration["ConnectionStrings:DefaultConnection"] = ConnectionString;
            Configuration["ConnectionStrings:ConnectionAfter"] = ConnectionStringAfter;

            DropAndCreateDatabase<ShopContext>(ConnectionString, _ => { });

            DropAndCreateDatabase<ShopContext>(ConnectionStringAfter);

            Checkpoint.TablesToIgnore = new[]
            {
                "__EFMigrationsHistory"
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