namespace GustosApp.Infraestructure;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Reflection.Emit;


public class GustosDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Gusto> Gustos { get; set; }
    public DbSet<Restriccion> Restricciones { get; set; }
    public DbSet<CondicionMedica> CondicionesMedicas { get; set; }

    public GustosDbContext(DbContextOptions<GustosDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relaciones muchos a muchos
        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Gustos)
            .WithMany(g => g.Usuarios)
            .UsingEntity(j => j.ToTable("UsuarioGustos"));

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Restricciones)
            .WithMany(r => r.Usuarios)
            .UsingEntity(j => j.ToTable("UsuarioRestricciones"));

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.CondicionesMedicas)
            .WithMany(p => p.Usuarios)
            .UsingEntity(j => j.ToTable("UsuarioCondicionesMedicas"));

        /* Ejemplo de constraint CHECK (solo para valores permitidos)
        modelBuilder.Entity<Restriccion>()
            .Property(r => r.Nombre)
            .HasMaxLength(50);
        */

        // Opcional: índices únicos
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.FirebaseUid)
            .IsUnique();

       

    }
}
