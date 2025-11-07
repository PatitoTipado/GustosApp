using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GustosApp.Infraestructure.Configurations
{
    public class UsuarioRestauranteVisitadoConfiguration : IEntityTypeConfiguration<UsuarioRestauranteVisitado>
    {
        public void Configure(EntityTypeBuilder<UsuarioRestauranteVisitado> builder)
        {
            builder.ToTable("UsuarioRestauranteVisitados");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Nombre)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(v => v.PlaceId)
                .HasMaxLength(450);

            builder.HasIndex(v => v.PlaceId);

            builder.HasOne(v => v.Usuario)
                .WithMany(u => u.Visitados)
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Restaurante)
                .WithMany()
                .HasForeignKey(v => v.RestauranteId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
