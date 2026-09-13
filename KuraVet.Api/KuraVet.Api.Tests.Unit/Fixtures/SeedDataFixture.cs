using KuraVet.Api.Data;
using KuraVet.Api.Models;

namespace KuraVet.Api.Tests.Unit.Fixtures
{
    public class SeedDataFixture
    {
        public string DatabaseName { get; } = Guid.NewGuid().ToString();

        public SeedDataFixture()
        {
            using var context = InMemoryDbContextFactory.CreateContext(DatabaseName);

            context.Tutores.Add(new Tutor { Id = 1, Nome = "Tutor Semente" });
            context.Pets.Add(new Pet { Id = 1, Nome = "Pet Semente", TutorId = 1 });

            context.SaveChanges();
        }

        public KuraVetDbContext CreateContext() => InMemoryDbContextFactory.CreateContext(DatabaseName);
    }
}
