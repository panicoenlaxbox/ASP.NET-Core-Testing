using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace XUnitTestProject1.Infrastructure.Fixtures
{
    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        private readonly string _fixture;
        private readonly Lazy<Type> _fixtureType;

        public ResetDatabaseAttribute(string fixture)
        {
            _fixture = fixture;
            _fixtureType = new Lazy<Type>(GetFixtureType);
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            ResetDatabase(false);            
        }

        public override void After(MethodInfo methodUnderTest)
        {
            ResetDatabase(true);
        }

        private void ResetDatabase(bool expected)
        {
            // Reflection is required due to xUnit does not inject class fixture in this attribute,
            // so there is no context about current test
            const string name = "ResetDatabaseAsync";
            var method = _fixtureType.Value.GetMethod(name, BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(bool) }, null);
            if (method != null)
            {
                ((Task) method.Invoke(null, new object[] {expected})).Wait();
            }
            else
            {
                method = _fixtureType.Value.GetMethod(name, BindingFlags.Public | BindingFlags.Static);
                ((Task)method.Invoke(null, null)).Wait();
            }
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