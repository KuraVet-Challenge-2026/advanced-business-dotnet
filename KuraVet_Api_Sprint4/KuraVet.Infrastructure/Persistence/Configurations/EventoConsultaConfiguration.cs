using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KuraVet.Infrastructure.Persistence.Configurations;

public sealed class EventoConsultaConfiguration : IEntityTypeConfiguration<EventoConsulta>
{
    public void Configure(EntityTypeBuilder<EventoConsulta> builder)
    {
        builder.ToTable("TB_KV_EVENTO_CONSULTA");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.TipoEvento)
            .IsRequired()
            .HasMaxLength(EventoConsulta.TipoTamanhoMaximo);

        builder.Property(e => e.DataEvento).IsRequired();

        builder.Property(e => e.Descricao)
            .IsRequired()
            .HasMaxLength(EventoConsulta.DescricaoTamanhoMaximo);

        builder.Property(e => e.VeterinarioResponsavel)
            .IsRequired()
            .HasMaxLength(EventoConsulta.VeterinarioTamanhoMaximo);

        builder.HasOne(e => e.Pet)
            .WithMany()
            .HasForeignKey(e => e.PetId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasIndex(e => e.PetId);
    }
}
