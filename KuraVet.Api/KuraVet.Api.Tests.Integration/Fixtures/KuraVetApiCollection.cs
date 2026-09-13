using Xunit;

namespace KuraVet.Api.Tests.Integration.Fixtures
{
    [CollectionDefinition(Name)]
    public class KuraVetApiCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
        public const string Name = "KuraVet API Integration Tests";
    }
}
