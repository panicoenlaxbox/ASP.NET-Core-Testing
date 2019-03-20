using Xunit;

namespace XUnitTestProject1.Infrastructure.Fixtures
{
    [CollectionDefinition("OmpCollection")]
    public class OmpCollection : ICollectionFixture<OmpFixture>
    {

    }
}