using System;
using System.Reflection;
using Xunit.Sdk;

namespace XUnitTestProject1
{
    public class DbComparer
    {
        private readonly string _sourceConnectionString;
        private readonly string _targetConnectionString;

        public DbComparer(string sourceConnectionString, string targetConnectionString)
        {
            _sourceConnectionString = sourceConnectionString;
            _targetConnectionString = targetConnectionString;
        }

        public bool Compare()
        {
            return false;
        }
    }

    public class DbComparerEntryResult
    {

    }

    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        private readonly bool _executeBefore;
        private readonly bool _executeAfter;

        public ResetDatabaseAttribute() : this(false, false)
        {

        }

        public ResetDatabaseAttribute(bool executeBefore, bool executeAfter)
        {
            _executeBefore = executeBefore;
            _executeAfter = executeAfter;
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
            HostFixture.ResetDatabaseAsync(after: true).Wait();

            if (!_executeAfter)
            {
                return;
            }

            ExecuteResource(methodUnderTest, after: true);
        }
    }
}