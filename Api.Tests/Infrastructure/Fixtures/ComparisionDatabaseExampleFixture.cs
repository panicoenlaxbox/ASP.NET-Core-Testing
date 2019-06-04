using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Respawn;
using Xunit.Abstractions;

namespace Api.Tests.Infrastructure.Fixtures
{
    public class ComparisionDatabaseExampleFixture : IntegrationFixtureBase
    {
        private static readonly Checkpoint ActualCheckpoint = new Checkpoint();
        private static readonly Checkpoint ExpectedCheckpoint = new Checkpoint();
        public static string ActualConnectionString { get; set; }
        public static string ExpectedConnectionString { get; set; }

        public ComparisionDatabaseExampleFixture(IMessageSink messageSink)
        {
            //var message = new DiagnosticMessage("I will appear in dotnet test");
            //messageSink.OnMessage(message);

            // We must read configuration after host has been built
            (ActualConnectionString, ExpectedConnectionString) = ParseConnectionStrings();

            UpdateConnectionStrings(ActualConnectionString, ExpectedConnectionString);

            DropAndCreateDatabase<ShopContext>(ActualConnectionString, _ => { });

            DropAndCreateDatabase<ShopContext>(ExpectedConnectionString);

            var tablesToIgnore = new[] {
                "__EFMigrationsHistory",
                "SchemaVersions",
                "sysdiagrams"
            };
            ActualCheckpoint.TablesToIgnore = tablesToIgnore;

            ExpectedCheckpoint.TablesToIgnore = tablesToIgnore;
        }

        protected (string, string) ParseConnectionStrings()
        {
            var unique = GetType().Name;
            return (
                string.Format(Configuration.GetConnectionString("DefaultConnection"), $"{unique}_actual"),
                string.Format(Configuration.GetConnectionString("ExpectedConnection"), $"{unique}_expected"));
        }

        private void UpdateConnectionStrings(string actualConnectionString, string expectedConnectionString)
        {
            Configuration["ConnectionStrings:DefaultConnection"] = actualConnectionString;
            Configuration["ConnectionStrings:ExpectedConnection"] = expectedConnectionString;
        }

        // ReSharper disable once UnusedMember.Global
        public static async Task ResetDatabaseAsync(bool expected = false)
        {
            var stopwatch = Stopwatch.StartNew();

            var connectionString = expected ? ExpectedConnectionString : ActualConnectionString;
            await ActualCheckpoint.Reset(connectionString);

            stopwatch.Stop();
            Logger.LogDebug($"{nameof(ResetDatabaseAsync)}, {connectionString} {stopwatch.Elapsed:g}");
        }
    }
}