using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit.Sdk;
using XUnitTestProject1.Helpers;

namespace XUnitTestProject1.Infrastructure.Fixtures
{
    public class EnhancedResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        private readonly string _fixture;
        private readonly bool _executeBefore;
        private readonly bool _executeAfter;
        private readonly string[] _schemas;
        private readonly IEnumerable<string> _tables;
        private readonly IEnumerable<string> _fields;
        private readonly bool _exclude;
        private readonly bool _count;
        private readonly Lazy<Type> _fixtureType;

        public EnhancedResetDatabaseAttribute(
            string fixture,
            bool executeBefore = false,
            bool executeAfter = false,
            string[] schemas = null,
            string[] tables = null,
            string[] fields = null,
            bool exclude = true,
            bool count = false)
        {
            _fixture = fixture;
            _executeBefore = executeBefore;
            _executeAfter = executeAfter;
            _schemas = schemas;
            _tables = tables;
            _fields = fields;
            _exclude = exclude;
            _count = count;
            _fixtureType = new Lazy<Type>(GetFixtureType);
        }

        public override async void Before(MethodInfo methodUnderTest)
        {
            await ResetDatabase(false);

            if (!_executeBefore)
            {
                return;
            }

            ExecuteEmbeddedResource(methodUnderTest, after: false);

            if (_executeAfter)
            {
                await ResetDatabase(true);
            }
        }

        public override void After(MethodInfo methodUnderTest)
        {
            if (_executeAfter)
            {
                ExecuteEmbeddedResource(methodUnderTest, after: true);

                var dbComparer = new DbComparer(
                    (string)GetFixturePropertyValue("ConnectionString"),
                    (string)GetFixturePropertyValue("ConnectionStringAfter"),
                    _schemas,
                    _tables,
                    _fields,
                    _exclude,
                    _count);
                var result = dbComparer.Compare();
                if (!result)
                {
                    var message = "Comparison failed\n";
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

        private object GetFixturePropertyValue(string propertyName)
        {
            return _fixtureType.Value.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static).GetValue(null, null);
        }

        private void ExecuteEmbeddedResource(MethodInfo methodUnderTest, bool after)
        {
            var assembly = typeof(EnhancedResetDatabaseAttribute).Assembly;
            var pattern = $@"{methodUnderTest.DeclaringType.Name}\.{methodUnderTest.Name}_{(after ? "after" : "before")}_?\d*\.sql$";
            var resources = new List<string>();
            foreach (var resource in assembly.GetManifestResourceNames())
            {
                if (Regex.IsMatch(resource, pattern))
                {
                    resources.Add(resource);
                }
            }

            if (!resources.Any())
            {
                throw new Exception($"There are no '{(after ? "after" : "before")}' resources to execute");
            }
            var connectionString = !after ?
                (string)GetFixturePropertyValue("ConnectionString") :
                (string)GetFixturePropertyValue("ConnectionStringAfter");
            SqlEmbeddedResourceExecutor.Execute(connectionString, assembly, resources);
        }

        private Task ResetDatabase(bool after)
        {
            // Reflection is required due to xUnit does not inject class fixture in this attribute,
            // there is no context about current test
            var method = _fixtureType.Value.GetMethod("ResetDatabaseAsync", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(bool) }, null);
            var task = (Task)method.Invoke(null, new object[] { after });
            //task.Wait();
            return task;
            //HostFixture.ResetDatabaseAsync(after).Wait();
        }

        private Type GetFixtureType()
        {
            var type = Type.GetType(_fixture);
            if (type != null)
            {
                return type;
            }

            return Assembly.GetExecutingAssembly().GetTypes().Single(t => t.Name == _fixture);
        }
    }
}