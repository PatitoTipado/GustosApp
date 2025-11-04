using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GustosApp.Infraestructure.Configurations
{
    public class RestauranteMenuConfiguration : IEntityTypeConfiguration<RestauranteMenu>
    {
        public void Configure(EntityTypeBuilder<RestauranteMenu> b)
        {
            b.ToTable("RestauranteMenus");
            b.HasKey(x => x.Id);
            b.Property(x => x.Moneda).HasMaxLength(8).IsRequired();
            b.Property(x => x.Json).IsRequired();

            b.HasOne(x => x.Restaurante)
             .WithMany()
             .HasForeignKey(x => x.RestauranteId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.RestauranteId).HasDatabaseName("IX_RestMenu_RestauranteId");
        }
    }
}