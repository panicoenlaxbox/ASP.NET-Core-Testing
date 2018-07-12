using System.Reflection;
using Xunit.Sdk;

namespace XUnitTestProject1
{
    public class ResetDatabase : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            HostFixture.ResetDatabaseAsync().Wait();
        }
    }
}