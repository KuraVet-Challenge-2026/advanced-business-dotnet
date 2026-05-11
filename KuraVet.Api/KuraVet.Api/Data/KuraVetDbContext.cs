using KuraVet.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace KuraVet.Api.Data
{
    public class KuraVetDbContext : DbContext
    {
        public KuraVetDbContext(DbContextOptions<KuraVetDbContext> options) : base(options)
        {
        }

        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<CheckinHistorico> CheckinsHistoricos { get; set; }
        public DbSet<EventoConsulta> EventosConsultas { get; set; }
    }
}