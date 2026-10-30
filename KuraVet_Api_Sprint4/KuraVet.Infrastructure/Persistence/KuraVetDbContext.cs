using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Infrastructure.Persistence;

public class KuraVetDbContext : DbContext
{
    public KuraVetDbContext(DbContextOptions<KuraVetDbContext> options) : base(options)
    {
    }

    public DbSet<Tutor> Tutores => Set<Tutor>();

    public DbSet<Pet> Pets => Set<Pet>();

    public DbSet<CheckinHistorico> CheckinsHistoricos => Set<CheckinHistorico>();

    public DbSet<EventoConsulta> EventosConsultas => Set<EventoConsulta>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KuraVetDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
