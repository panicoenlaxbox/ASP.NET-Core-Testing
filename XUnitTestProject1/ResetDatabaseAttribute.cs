using System.Reflection;
using Xunit.Sdk;

namespace XUnitTestProject1
{
    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        private readonly bool _executeResource;

        public ResetDatabaseAttribute() : this(false)
        {

        }

        public ResetDatabaseAttribute(bool executeResource)
        {
            _executeResource = executeResource;
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            HostFixture.ResetDatabaseAsync().Wait();

            if (!_executeResource)
            {
                return;
            }

            var assembly = typeof(ResetDatabaseAttribute).Assembly;

            var resource = $"{assembly.GetName().Name}.Sql.{methodUnderTest.DeclaringType.Name}.{methodUnderTest.Name}.sql";

            var a = assembly.GetManifestResourceNames();

            SqlResourceExecutor.Execute(HostFixture.ConnectionString, assembly, resource);
        }
    }
}