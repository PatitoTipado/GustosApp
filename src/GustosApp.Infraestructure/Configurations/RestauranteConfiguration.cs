
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
            builder.Property(r => r.PropietarioUid).HasMaxLength(128).IsRequired(false); 
            builder.Property(r => r.Nombre).IsRequired().HasMaxLength(160);
            builder.Property(r => r.NombreNormalizado).HasMaxLength(160);
            builder.Property(r => r.Direccion).HasMaxLength(300);
            builder.Property(r => r.HorariosJson);
            builder.Property(r => r.CreadoUtc);
            builder.Property(r => r.ActualizadoUtc);


            builder.HasIndex(r => r.PropietarioUid).IsUnique().HasFilter("[PropietarioUid] IS NOT NULL AND [PropietarioUid] <> ''");
            builder.HasIndex(r => r.NombreNormalizado).IsUnique().HasFilter("[NombreNormalizado] IS NOT NULL AND [NombreNormalizado] <> ''");
            builder.HasIndex(r => new { r.Latitud, r.Longitud });
        }
    }
}
