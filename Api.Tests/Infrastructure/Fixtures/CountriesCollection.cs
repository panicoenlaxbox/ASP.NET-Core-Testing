using Xunit;

namespace Api.Tests.Infrastructure.Fixtures
{
    [CollectionDefinition("CountriesCollection")]
    public class CountriesCollection : ICollectionFixture<CountriesFixture>
    {

    }
}