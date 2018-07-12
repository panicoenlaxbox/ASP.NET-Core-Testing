using Xunit;

namespace XUnitTestProject1
{
    [CollectionDefinition("HostCollectionFixture")]
    public class HostCollectionFixture:ICollectionFixture<HostFixture>
    {
        
    }
}