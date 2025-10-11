
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
            b.HasKey(x => x.Id);

            b.Property(x => x.PropietarioUid).HasMaxLength(128).IsRequired();
            b.Property(x => x.Nombre).HasMaxLength(160).IsRequired();
            b.Property(x => x.NombreNormalizado).HasMaxLength(160).IsRequired();
            b.Property(x => x.Direccion).HasMaxLength(300).IsRequired();
            b.Property(x => x.HorariosJson).HasColumnType("nvarchar(max)").IsRequired();

            b.Property(x => x.Latitud);
            b.Property(x => x.Longitud);

            b.Property(x => x.CreadoUtc).HasColumnType("datetime2");
            b.Property(x => x.ActualizadoUtc).HasColumnType("datetime2");

            // ====== V2 ======
            b.Property(x => x.Tipo)
             .HasConversion<string>() 
             .HasMaxLength(40)
             .IsRequired();

            b.Property(x => x.ImagenUrl).HasMaxLength(500);
            b.Property(x => x.Valoracion).HasColumnType("decimal(3,2)"); 

            b.HasIndex(x => x.PropietarioUid).IsUnique().HasDatabaseName("UX_Restaurantes_PropietarioUid");
            b.HasIndex(x => x.NombreNormalizado).IsUnique().HasDatabaseName("UX_Restaurantes_NombreNormalizado");

            b.HasMany(x => x.Platos)
             .WithOne(x => x.Restaurante)
             .HasForeignKey(x => x.RestauranteId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

