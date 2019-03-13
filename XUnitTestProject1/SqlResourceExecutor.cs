using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace XUnitTestProject1
{
    public class SqlResourceExecutor
    {
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