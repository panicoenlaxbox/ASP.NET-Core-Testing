using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace XUnitTestProject1
{
    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        private readonly string _collectionFixture;
        private readonly bool _executeBefore;
        private readonly bool _executeAfter;
        private readonly string[] _schemas;
        private readonly IEnumerable<string> _tables;
        private readonly IEnumerable<string> _fields;
        private readonly bool _exclude;
        private readonly bool _count;

        public ResetDatabaseAttribute(
            string collectionFixture,
            bool executeBefore = false,
            bool executeAfter = false,
            string[] schemas = null,
            string[] tables = null,
            string[] fields = null,
            bool exclude = true,
            bool count = false)
        {
            _collectionFixture = collectionFixture;
            _executeBefore = executeBefore;
            _executeAfter = executeAfter;
            _schemas = schemas;
            _tables = tables;
            _fields = fields;
            _exclude = exclude;
            _count = count;
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            ResetDatabase(false);

            if (!_executeBefore)
            {
                return;
            }

            ExecuteResource(methodUnderTest, after: false);

            if (_executeAfter)
            {
                ResetDatabase(true);
            }
        }

        private object GetCollectionFixturePropertyValue(string propertyName)
        {
            return Type.GetType(_collectionFixture).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static).GetValue(null, null);
        }

        private void ExecuteResource(MethodInfo methodUnderTest, bool after)
        {
            var assembly = typeof(ResetDatabaseAttribute).Assembly;
            var pattern = $@"{methodUnderTest.DeclaringType.Name}\.{methodUnderTest.Name}_{(after ? "after" : "before")}_?\d*\.sql$";
            var resources = new List<string>();
            foreach (var resource in assembly.GetManifestResourceNames())
            {
                if (Regex.IsMatch(resource, pattern))
                {
                    resources.Add(resource);
                }
            }
            var connectionString = !after ?
                (string)GetCollectionFixturePropertyValue("ConnectionString") :
                (string)GetCollectionFixturePropertyValue("ConnectionStringAfter");
            SqlResourceExecutor.Execute(connectionString, assembly, resources);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            if (_executeAfter)
            {
                ExecuteResource(methodUnderTest, after: true);

                var dbComparer = new DbComparer(
                    (string)GetCollectionFixturePropertyValue("ConnectionString"),
                    (string)GetCollectionFixturePropertyValue("ConnectionStringAfter"),
                    _schemas,
                    _tables,
                    _fields,
                    _exclude,
                    _count);
                var result = dbComparer.Compare();
                if (!result)
                {
                    var message = "Db checksums are not equal\n";
                    message += $"Tables matched {result.Entries.Count(e => e.Match)}\n";
                    message += $"Tables not matched {result.Entries.Count(e => !e.Match)}\n";
                    foreach (var entry in result.Entries.Where(e => !e.Match))
                    {
                        message += $"{entry}\n";
                    }

                    throw new Exception(message);
                }
            }
        }

        private void ResetDatabase(bool after)
        {
            // Reflection is required due to xUnit does not inject class fixture in this attribute,
            // there is no context about current test
            var type = Type.GetType(_collectionFixture);
            var method = type.GetMethod("ResetDatabaseAsync", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(bool) }, null);
            var task = (Task)method.Invoke(null, new object[] { after });
            task.Wait();
            //HostFixture.ResetDatabaseAsync(after).Wait();
        }
    }
}