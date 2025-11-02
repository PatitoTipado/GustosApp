using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GustosApp.Infraestructure.Configurations
{
    public class RestauranteImagenConfiguration : IEntityTypeConfiguration<RestauranteImagen>
    {
        public void Configure(EntityTypeBuilder<RestauranteImagen> b)
        {
            b.ToTable("RestauranteImagenes");
            b.HasKey(x => x.Id);
            b.Property(x => x.Url).IsRequired().HasMaxLength(1024);
            b.Property(x => x.Tipo).HasConversion<int>().IsRequired();

            b.HasOne(x => x.Restaurante)
             .WithMany() // no navegar desde Restaurante para no tocar contrato actual
             .HasForeignKey(x => x.RestauranteId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.RestauranteId, x.Tipo, x.Orden })
             .HasDatabaseName("IX_RestImagen_Rest_Tipo_Orden");
        }
    }
}