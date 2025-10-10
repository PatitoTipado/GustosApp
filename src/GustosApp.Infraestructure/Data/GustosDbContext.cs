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
    public DbSet<Grupo> Grupos { get; set; }
    public DbSet<MiembroGrupo> MiembrosGrupos { get; set; }
    public DbSet<InvitacionGrupo> InvitacionesGrupos { get; set; }
    
    public DbSet<Restaurante> Restaurantes { get; set; }

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


        modelBuilder.Entity<Usuario>()
           .HasIndex(u => u.Email)
           .IsUnique();

        modelBuilder.Entity<Usuario>()
           .HasIndex(u => u.IdUsuario)
           .IsUnique();

        modelBuilder.Entity<Gusto>()
            .HasMany(g => g.Tags)
            .WithMany()
            .UsingEntity(j => j.ToTable("GustoTags"));

        modelBuilder.Entity<Restriccion>()
            .HasMany(r => r.TagsProhibidos)
            .WithMany()
            .UsingEntity(j => j.ToTable("RestriccionTags"));

        modelBuilder.Entity<CondicionMedica>()
            .HasMany(c => c.TagsCriticos)
            .WithMany()
            .UsingEntity(j => j.ToTable("CondicionMedicaTags"));

       
        modelBuilder.Entity<Tag>()
            .Property(t => t.Tipo)
            .HasConversion<string>();

        modelBuilder.Entity<Tag>()
        .Ignore(t => t.NombreNormalizado);


        // Configuración de relaciones para grupos
        modelBuilder.Entity<Grupo>()
            .HasOne(g => g.Administrador)
            .WithMany(u => u.GruposAdministrados)
            .HasForeignKey(g => g.AdministradorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MiembroGrupo>()
            .HasOne(m => m.Grupo)
            .WithMany(g => g.Miembros)
            .HasForeignKey(m => m.GrupoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MiembroGrupo>()
            .HasOne(m => m.Usuario)
            .WithMany(u => u.MiembrosGrupos)
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InvitacionGrupo>()
            .HasOne(i => i.Grupo)
            .WithMany(g => g.Invitaciones)
            .HasForeignKey(i => i.GrupoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InvitacionGrupo>()
            .HasOne(i => i.UsuarioInvitado)
            .WithMany(u => u.InvitacionesRecibidas)
            .HasForeignKey(i => i.UsuarioInvitadoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InvitacionGrupo>()
            .HasOne(i => i.UsuarioInvitador)
            .WithMany(u => u.InvitacionesEnviadas)
            .HasForeignKey(i => i.UsuarioInvitadorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices únicos para grupos
        modelBuilder.Entity<Grupo>()
            .HasIndex(g => g.CodigoInvitacion)
            .IsUnique()
            .HasFilter("[CodigoInvitacion] IS NOT NULL");

        modelBuilder.Entity<MiembroGrupo>()
            .HasIndex(m => new { m.GrupoId, m.UsuarioId })
            .IsUnique();

        // Configurar enum para EstadoInvitacion
        modelBuilder.Entity<InvitacionGrupo>()
            .Property(i => i.Estado)
            .HasConversion<int>();

        modelBuilder.Entity<Gusto>().HasData(
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Nombre = "Pizza" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Nombre = "Sushi" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), Nombre = "Pastas" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), Nombre = "Milanesa con papas fritas" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), Nombre = "Empanadas" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111116"), Nombre = "Paella" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111117"), Nombre = "Tacos" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111118"), Nombre = "Choripán" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111119"), Nombre = "Risotto" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111120"), Nombre = "Guiso de lentejas" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111121"), Nombre = "Pizza napolitana" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111122"), Nombre = "Ñoquis" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111123"), Nombre = "Ravioles" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111124"), Nombre = "Ensalada César" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111125"), Nombre = "Ramen japonés" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111126"), Nombre = "Tarta de jamón y queso" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111127"), Nombre = "Ceviche peruano" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111128"), Nombre = "Ensaladas" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111129"), Nombre = "Pollo frito" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111130"), Nombre = "Papas fritas" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111131"), Nombre = "Kebab" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111132"), Nombre = "Flan" },
            new Gusto { Id = Guid.Parse("11111111-1111-1111-1111-111111111133"), Nombre = "Helado" }
            );

        modelBuilder.Entity<Restriccion>().HasData(
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222221"), Nombre = "Lactosa" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Nombre = "Fructosa" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222223"), Nombre = "Gluten no celíaco" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222224"), Nombre = "Cafeína" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222225"), Nombre = "Maní" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222226"), Nombre = "Pescado" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222227"), Nombre = "Chocolate" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222228"), Nombre = "Gluten" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222229"), Nombre = "Mariscos" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222230"), Nombre = "Frutos secos" },
            new Restriccion { Id = Guid.Parse("22222222-2222-2222-2222-222222222231"), Nombre = "Mostaza" });


        modelBuilder.Entity<CondicionMedica>().HasData(
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333331"), Nombre = "Diabetes" },
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333332"), Nombre = "Hipertensión" },
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Nombre = "Obesidad" },
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333334"), Nombre = "Gastritis" },
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333335"), Nombre = "Hígado graso" },
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333336"), Nombre = "Anemia" },
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333337"), Nombre = "Síndrome del intestino irritable" },
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333338"), Nombre = "Insuficiencia renal" },
            new CondicionMedica { Id = Guid.Parse("33333333-3333-3333-3333-333333333339"), Nombre = "Colesterol alto" });
    
    modelBuilder.ApplyConfiguration(new GustosApp.Infraestructure.Configurations.RestauranteConfiguration());

    }
}
