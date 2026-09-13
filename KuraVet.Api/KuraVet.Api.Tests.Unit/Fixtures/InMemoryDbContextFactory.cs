using KuraVet.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Api.Tests.Unit.Fixtures
{
    public static class InMemoryDbContextFactory
    {
        public static KuraVetDbContext CreateContext(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<KuraVetDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            return new KuraVetDbContext(options);
        }
    }
}
