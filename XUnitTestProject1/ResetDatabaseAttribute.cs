using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace XUnitTestProject1
{

    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        private readonly bool _executeBefore;
        private readonly bool _executeAfter;
        private readonly string[] _schemas;
        private readonly IEnumerable<string> _tables;
        private readonly IEnumerable<string> _fields;
        private readonly bool _exclude;
        private readonly bool _count;
        private readonly bool _resetAfter;

        public ResetDatabaseAttribute(
            bool executeBefore = false, 
            bool executeAfter = false,
            string[] schemas = null, 
            string[] tables = null, 
            string[] fields = null,
            bool exclude = true,
            bool count = false, 
            bool resetAfter = true)
        {
            _executeBefore = executeBefore;
            _executeAfter = executeAfter;
            _schemas = schemas;
            _tables = tables;
            _fields = fields;
            _exclude = exclude;
            _count = count;
            _resetAfter = resetAfter;
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            HostFixture.ResetDatabaseAsync().Wait();

            if (!_executeBefore)
            {
                return;
            }

            ExecuteResource(methodUnderTest, after: false);
        }

        private static void ExecuteResource(MethodInfo methodUnderTest, bool after)
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
            var connectionString = after ? HostFixture.ConnectionStringAfter : HostFixture.ConnectionString;
            SqlResourceExecutor.Execute(connectionString, assembly, resources);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            if (_executeAfter)
            {
                ExecuteResource(methodUnderTest, after: true);

                var dbComparer = new DbComparer(
                    HostFixture.ConnectionString, 
                    HostFixture.ConnectionStringAfter,
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

                    // We must make sure that the next test is not dirty, because if we throw an exception, nothing else of this method will be executed
                    if (_resetAfter)
                    {
                        HostFixture.ResetDatabaseAsync(after: true).Wait();
                    }

                    throw new Exception(message);
                }
            }

            HostFixture.ResetDatabaseAsync(after: true).Wait();
        }
    }
}