
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GustosApp.Infraestructure.Configurations
{
    public class RestauranteConfiguration : IEntityTypeConfiguration<Restaurante>
    {
        public void Configure(EntityTypeBuilder<Restaurante> builder)
        {
            builder.ToTable("Restaurantes");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.PropietarioUid).IsRequired().HasMaxLength(128);
            builder.Property(r => r.Nombre).IsRequired().HasMaxLength(160);
            builder.Property(r => r.NombreNormalizado).IsRequired().HasMaxLength(160);
            builder.Property(r => r.Direccion).IsRequired().HasMaxLength(300);
            builder.Property(r => r.HorariosJson).IsRequired();
            builder.Property(r => r.CreadoUtc).IsRequired();
            builder.Property(r => r.ActualizadoUtc).IsRequired();

            builder.HasIndex(r => r.PropietarioUid).IsUnique();
            builder.HasIndex(r => r.NombreNormalizado).IsUnique();
            builder.HasIndex(r => new { r.Latitud, r.Longitud });
        }
    }
}
