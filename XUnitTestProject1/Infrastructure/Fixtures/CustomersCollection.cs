using Xunit;

namespace XUnitTestProject1.Infrastructure.Fixtures
{
    [CollectionDefinition("CustomersCollection")]
    public class CustomersCollection : ICollectionFixture<CustomersFixture>
    {

    }
}