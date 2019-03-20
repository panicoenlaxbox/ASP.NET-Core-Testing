using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Sdk;
using XUnitTestProject1.Helpers;

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
            ResetDatabase(true);
        }

        //public override void After(MethodInfo methodUnderTest)
        //{
        //    if (_executeAfter)
        //    {
        //        ExecuteEmbeddedResource(methodUnderTest, after: true).Wait();

        //        var dbComparer = new DbComparer(
        //            (string)GetFixturePropertyValue("ConnectionString"),
        //            (string)GetFixturePropertyValue("ConnectionStringAfter"),
        //            _schemas,
        //            _tables,
        //            _fields,
        //            _exclude,
        //            _count);
        //        var result = dbComparer.Compare();
        //        if (!result)
        //        {
        //            var message = "Comparison failed\n";
        //            message += $"Tables matched {result.Entries.Count(e => e.Match)}\n";
        //            message += $"Tables not matched {result.Entries.Count(e => !e.Match)}\n";
        //            foreach (var entry in result.Entries.Where(e => !e.Match))
        //            {
        //                message += $"{entry}\n";
        //            }

        //            throw new Exception(message);
        //        }
        //    }
        //}

        private object GetFixturePropertyValue(string propertyName)
        {
            return _fixtureType.Value.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static).GetValue(null, null);
        }

        private void ResetDatabase(bool after)
        {
            // Reflection is required due to xUnit does not inject class fixture in this attribute,
            // there is no context about current test
            var method = _fixtureType.Value.GetMethod("ResetDatabaseAsync", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(bool) }, null);
            var task = (Task)method.Invoke(null, new object[] { after });
            task.Wait();
            //return task;
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