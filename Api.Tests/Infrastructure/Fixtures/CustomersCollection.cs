using Xunit;

namespace Api.Tests.Infrastructure.Fixtures
{
    [CollectionDefinition("CustomersCollection")]
    public class CustomersCollection : ICollectionFixture<CustomersFixture>
    {

    }
}