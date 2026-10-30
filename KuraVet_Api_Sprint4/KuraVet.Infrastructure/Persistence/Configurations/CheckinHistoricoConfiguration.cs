using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KuraVet.Infrastructure.Persistence.Configurations;

public sealed class CheckinHistoricoConfiguration : IEntityTypeConfiguration<CheckinHistorico>
{
    public void Configure(EntityTypeBuilder<CheckinHistorico> builder)
    {
        builder.ToTable("TB_KV_CHECKIN_HISTORICO");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.DataCheckin).IsRequired();
        builder.Property(c => c.FrequenciaRespiratoria).IsRequired();
        builder.Property(c => c.TempoPreenchimentoCapilar).IsRequired();

        builder.Property(c => c.CorMucosa)
            .IsRequired()
            .HasMaxLength(CheckinHistorico.TextoTamanhoMaximo);

        builder.Property(c => c.NivelHidratacao)
            .IsRequired()
            .HasMaxLength(CheckinHistorico.TextoTamanhoMaximo);

        builder.Property(c => c.NivelRiscoIA)
            .HasMaxLength(20);

        builder.HasOne(c => c.Pet)
            .WithMany(p => p.Checkins)
            .HasForeignKey(c => c.PetId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasIndex(c => c.PetId);
    }
}
