using Api.Entities;

namespace Api.Tests.Infrastructure.TestData
{
    public static class CustomerObjectMother
    {
        public static Customer CustomerWithName()
        {
            return Build.Customer()
                .WithName("Customer 1");
        }
    }
}