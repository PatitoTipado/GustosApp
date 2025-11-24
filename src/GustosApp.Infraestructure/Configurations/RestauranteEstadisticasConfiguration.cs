using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GustosApp.Infraestructure.Configurations
{
    public class RestauranteEstadisticasConfiguration : IEntityTypeConfiguration<RestauranteEstadisticas>
    {
        public void Configure(EntityTypeBuilder<RestauranteEstadisticas> builder)
        {
            builder.ToTable("RestauranteEstadisticas");

            builder.HasKey(e => e.RestauranteId);

            builder.Property(e => e.TotalTop3Individual)
                .IsRequired();

            builder.Property(e => e.TotalTop3Grupo)
                .IsRequired();

            builder.Property(e => e.TotalVisitasPerfil)
                .IsRequired();

            builder.Property(e => e.FechaCreacionUtc)
                .IsRequired();

            builder.Property(e => e.UltimaActualizacionUtc)
                .IsRequired();

            builder.HasOne(e => e.Restaurante)
                .WithOne(r => r.Estadisticas)
                .HasForeignKey<RestauranteEstadisticas>(e => e.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
