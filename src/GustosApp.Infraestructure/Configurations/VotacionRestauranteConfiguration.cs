using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Configurations
{
    public class VotacionRestauranteConfiguration : IEntityTypeConfiguration<VotacionRestaurante>
    {
        public void Configure(EntityTypeBuilder<VotacionRestaurante> builder)
        {
            builder.ToTable("VotacionRestaurantes");

            builder.HasKey(vr => vr.Id);

            builder.Property(vr => vr.VotacionId)
                .IsRequired();

            builder.Property(vr => vr.RestauranteId)
                .IsRequired();

            // Relación con VotacionGrupo (1:N)
            builder.HasOne(vr => vr.Votacion)
                .WithMany(v => v.RestaurantesCandidatos)
                .HasForeignKey(vr => vr.VotacionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Restaurante (1:N)
            builder.HasOne(vr => vr.Restaurante)
                .WithMany()
                .HasForeignKey(vr => vr.RestauranteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice opcional si querés evitar duplicados:
            builder.HasIndex(vr => new { vr.VotacionId, vr.RestauranteId })
                .IsUnique();
        }
    }

}