using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace XUnitTestProject1
{

    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        private readonly bool _executeBefore;
        private readonly bool _executeAfter;
        private readonly IEnumerable<string> _tablesToExclude;
        private readonly IEnumerable<string> _fieldsToExclude;

        public ResetDatabaseAttribute() : this(false, false, null, null)
        {

        }

        public ResetDatabaseAttribute(bool executeBefore, bool executeAfter, string[] tablesToExclude = null, string[] fieldsToExclude = null)
        {
            _executeBefore = executeBefore;
            _executeAfter = executeAfter;
            _tablesToExclude = tablesToExclude;
            _fieldsToExclude = fieldsToExclude;
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
            var resource =
                $"{assembly.GetName().Name}.Sql.{methodUnderTest.DeclaringType.Name}.{methodUnderTest.Name}_{(after ? "after" : "before")}.sql";
            var connectionString = after ? HostFixture.ConnectionStringAfter : HostFixture.ConnectionString;
            SqlResourceExecutor.Execute(connectionString, assembly, resource);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            if (_executeAfter)
            {
                ExecuteResource(methodUnderTest, after: true);
                var dbComparer = new DbComparer(HostFixture.ConnectionString, HostFixture.ConnectionStringAfter,
                    _tablesToExclude, _fieldsToExclude);
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

                    throw new XunitException(message);
                }
            }

            HostFixture.ResetDatabaseAsync(after: true).Wait();
        }
    }
}