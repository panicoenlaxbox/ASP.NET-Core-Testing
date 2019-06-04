using Api.Entities;

namespace Api.Tests.Infrastructure.TestData
{
    public static class Given
    {
        public static class A
        {
            public static Customer CustomerWithName()
            {
                return CustomerObjectMother.CustomerWithName();
            }
        }
    }
}