using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GustosApp.Infraestructure.Configurations
{
    public class RestauranteConfiguration : IEntityTypeConfiguration<Restaurante>
    {
        public void Configure(EntityTypeBuilder<Restaurante> b)
        {
            b.ToTable("Restaurantes");

            b.HasKey(r => r.Id);

            b.Property(r => r.PropietarioUid)
                .HasMaxLength(128)
                .IsRequired();

            b.Property(r => r.Nombre)
                .HasMaxLength(160)
                .IsRequired();

            b.Property(r => r.NombreNormalizado)
                .HasMaxLength(160);

            b.Property(r => r.Direccion)
                .HasMaxLength(300)
                .IsRequired();

            b.Property(r => r.HorariosJson)
                .IsRequired();

            b.Property(r => r.ImagenUrl)
                .HasMaxLength(500);

            b.Property(r => r.Tipo)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            b.Property(r => r.Valoracion)
                .HasPrecision(3,2);

            b.HasIndex(r => new { r.Latitud, r.Longitud })
             .HasDatabaseName("IX_Restaurantes_Latitud_Longitud");

            
            b.HasIndex(r => r.PropietarioUid)
             .HasDatabaseName("IX_Restaurantes_PropietarioUid");

            b.HasIndex(r => r.NombreNormalizado)
             .IsUnique()
             .HasDatabaseName("UX_Restaurantes_NombreNormalizado");
        }
    }
}

