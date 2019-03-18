using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace XUnitTestProject1.Helpers
{
    public class SqlEmbeddedResourceExecutor
    {
        public static void Execute(string connectionString, Assembly assembly, IEnumerable<string> resources)
        {
            foreach (var resource in resources.OrderBy(r => r))
            {
                Execute(connectionString, assembly, resource);
            }
        }

        public static void Execute(string connectionString, Assembly assembly, string resource)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"{resource} not found", nameof(resource));
                }
                using (var reader = new StreamReader(stream))
                {
                    var commandText = reader.ReadToEnd();
                    using (var connection = new SqlConnection(connectionString))
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
}