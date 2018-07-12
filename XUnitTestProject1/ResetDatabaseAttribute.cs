using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace XUnitTestProject1
{
    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        private readonly string _connectionString;

        public ResetDatabaseAttribute()
        {

        }

        public ResetDatabaseAttribute(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            HostFixture.ResetDatabaseAsync().Wait();

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                return;
            }

            var assembly = typeof(ResetDatabaseAttribute).Assembly;

            var resource = $"{assembly.GetName().Name}.{methodUnderTest.Name}.sql";

            using (var stream = assembly.GetManifestResourceStream(resource))
            using (var reader = new StreamReader(stream))
            {
                var commandText = reader.ReadToEnd();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}