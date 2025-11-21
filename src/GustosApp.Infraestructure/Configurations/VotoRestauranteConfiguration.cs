using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GustosApp.Infraestructure.Configurations
{
    public class VotoRestauranteConfiguration : IEntityTypeConfiguration<VotoRestaurante>
    {
        public void Configure(EntityTypeBuilder<VotoRestaurante> builder)
        {
            builder.ToTable("Votos");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.VotacionId)
                .IsRequired();

            builder.Property(v => v.UsuarioId)
                .IsRequired();

            builder.Property(v => v.RestauranteId)
                .IsRequired();

            builder.Property(v => v.FechaVoto)
                .IsRequired();

            builder.Property(v => v.Comentario)
                .HasMaxLength(500);

            builder.HasOne(v => v.Votacion)
                .WithMany(vo => vo.Votos)
                .HasForeignKey(v => v.VotacionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Restaurante)
                .WithMany()
                .HasForeignKey(v => v.RestauranteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un usuario solo puede votar una vez por votación
            builder.HasIndex(v => new { v.VotacionId, v.UsuarioId })
                .IsUnique();
        }
    }
}
