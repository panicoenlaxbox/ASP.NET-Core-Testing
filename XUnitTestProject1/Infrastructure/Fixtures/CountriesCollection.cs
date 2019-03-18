using Xunit;

namespace XUnitTestProject1.Infrastructure.Fixtures
{
    [CollectionDefinition("CountriesCollection")]
    public class CountriesCollection : ICollectionFixture<CountriesFixture>
    {

    }
}