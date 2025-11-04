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

            b.Property(r => r.PlaceId)
                .HasMaxLength(450)
                .IsRequired(false);

            b.Property(r => r.Categoria)
                .HasMaxLength(80);

            b.Property(r => r.ImagenUrl)
                .HasMaxLength(2048);

            b.Property(r => r.WebUrl)
                .HasMaxLength(2048);

            b.Property(r => r.PrimaryType)
                .HasMaxLength(80)
                .IsRequired()
                .HasDefaultValue("restaurant");

            b.Property(r => r.TypesJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired()
                .HasDefaultValue("[]");

            b.HasIndex(r => r.PrimaryType);

            b.Property(r => r.Valoracion)
                .HasPrecision(3, 2);

            b.HasIndex(r => r.NombreNormalizado)
                .HasDatabaseName("IX_Restaurantes_NombreNormalizado")
                .IsUnique(false);

            b.HasIndex(r => r.PlaceId)
                .HasDatabaseName("UX_Restaurantes_PlaceId")
                .IsUnique()
                .HasFilter("[PlaceId] IS NOT NULL AND [PlaceId] <> ''");
            b.HasIndex(r => new { r.Latitud, r.Longitud })
                .HasDatabaseName("IX_Restaurantes_Latitud_Longitud");
        }
    }
}