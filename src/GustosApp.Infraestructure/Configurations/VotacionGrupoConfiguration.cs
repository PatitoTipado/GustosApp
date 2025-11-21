using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GustosApp.Infraestructure.Configurations
{
    public class VotacionGrupoConfiguration : IEntityTypeConfiguration<VotacionGrupo>
    {
        public void Configure(EntityTypeBuilder<VotacionGrupo> builder)
        {
            builder.ToTable("Votaciones");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.GrupoId)
                .IsRequired();

            builder.Property(v => v.FechaInicio)
                .IsRequired();

            builder.Property(v => v.Estado)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(v => v.Descripcion)
                .HasMaxLength(500);

            builder.HasOne(v => v.Grupo)
                .WithMany()
                .HasForeignKey(v => v.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.RestauranteGanador)
                .WithMany()
                .HasForeignKey(v => v.RestauranteGanadorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(v => v.Votos)
                .WithOne(vo => vo.Votacion)
                .HasForeignKey(vo => vo.VotacionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(v => new { v.GrupoId, v.Estado });
        }
    }
}
