using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KuraVet.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("TB_KV_USUARIO");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(Usuario.NomeTamanhoMaximo);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(Usuario.EmailTamanhoMaximo);

        builder.Property(u => u.SenhaHash)
            .IsRequired()
            .HasMaxLength(Usuario.SenhaHashTamanhoMaximo);

        builder.Property(u => u.Perfil)
            .IsRequired()
            .HasMaxLength(Usuario.PerfilTamanhoMaximo);

        builder.Property(u => u.DataCriacao).IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
