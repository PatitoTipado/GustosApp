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

    public DbSet<ReviewRestaurante> ReviewsRestaurantes { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<RestaurantePlato> RestaurantePlatos { get; set; }

    public GustosDbContext(DbContextOptions<GustosDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurante>()
            .HasMany(r => r.Reviews)
                .WithOne()
                .HasForeignKey(r => r.RestauranteId)
                .OnDelete(DeleteBehavior.Cascade);
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

        // Gusto ↔ Tag
        modelBuilder.Entity<Gusto>()
            .HasMany(g => g.Tags)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "GustoTags",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagsId"),
                j => j.HasOne<Gusto>().WithMany().HasForeignKey("GustosId"),
                j =>
                {
                    j.HasKey("GustosId", "TagsId");
                    j.ToTable("GustoTags");
                });

        // Restriccion ↔ Tag
        modelBuilder.Entity<Restriccion>()
            .HasMany(r => r.TagsProhibidos)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "RestriccionTags",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagsId"),
                j => j.HasOne<Restriccion>().WithMany().HasForeignKey("RestriccionesId"),
                j =>
                {
                    j.HasKey("RestriccionesId", "TagsId");
                    j.ToTable("RestriccionTags");
                });

        // CondicionMedica ↔ Tag
        modelBuilder.Entity<CondicionMedica>()
            .HasMany(c => c.TagsCriticos)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "CondicionMedicaTags",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagsId"),
                j => j.HasOne<CondicionMedica>().WithMany().HasForeignKey("CondicionesMedicasId"),
                j =>
                {
                    j.HasKey("CondicionesMedicasId", "TagsId");
                    j.ToTable("CondicionMedicaTags");
                });

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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GustosDbContext).Assembly);

        modelBuilder.Entity<RestaurantePlato>(b =>
    {
        b.ToTable("RestaurantePlatos");

        b.HasKey(x => new { x.RestauranteId, x.Plato });

        b.Property(x => x.Plato)
         .HasConversion<string>()
         .HasMaxLength(50);

        b.HasOne(x => x.Restaurante)
         .WithMany(r => r.Platos)
         .HasForeignKey(x => x.RestauranteId)
         .OnDelete(DeleteBehavior.Cascade);
    });



        var tags = new[]
  {
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Nombre = "Gluten", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Nombre = "Lácteo", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), Nombre = "Azúcar", Tipo = TipoTag.Nutriente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), Nombre = "Sal", Tipo = TipoTag.Nutriente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), Nombre = "Mariscos", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111116"), Nombre = "Carne roja", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111117"), Nombre = "Carne blanca", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111118"), Nombre = "Vegetal", Tipo = TipoTag.Categoria },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111119"), Nombre = "Fruta", Tipo = TipoTag.Categoria },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111120"), Nombre = "Frito", Tipo = TipoTag.Categoria },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111121"), Nombre = "Picante", Tipo = TipoTag.Categoria },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111122"), Nombre = "Procesado", Tipo = TipoTag.Categoria },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111123"), Nombre = "Pescado", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111124"), Nombre = "Grasa", Tipo = TipoTag.Nutriente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111125"), Nombre = "Cafeína", Tipo = TipoTag.Nutriente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111126"), Nombre = "Harina", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111127"), Nombre = "Huevos", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111128"), Nombre = "Frutos secos", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111129"), Nombre = "Soja", Tipo = TipoTag.Ingrediente },
    new Tag { Id = Guid.Parse("11111111-1111-1111-1111-111111111130"), Nombre = "Alcohol", Tipo = TipoTag.Nutriente }
};
        modelBuilder.Entity<Tag>().HasData(tags);

        // --- GUSTOS / PLATOS (31) ---
        var gustos = Enumerable.Range(1, 31)
            .Select(i => new Gusto
            {
                Id = Guid.Parse($"22222222-0001-0001-0001-{i:D12}"),
                Nombre = i switch
                {
                    1 => "Pizza",
                    2 => "Sushi",
                    3 => "Paella",
                    4 => "Milanesa con papas",
                    5 => "Tacos",
                    6 => "Ensalada César",
                    7 => "Ramen japonés",
                    8 => "Empanadas",
                    9 => "Ceviche",
                    10 => "Helado",
                    11 => "Hamburguesa",
                    12 => "Papas fritas",
                    13 => "Pollo grillado",
                    14 => "Kebab",
                    15 => "Ensalada verde",
                    16 => "Asado",
                    17 => "Sopa de verduras",
                    18 => "Panqueques",
                    19 => "Café con leche",
                    20 => "Jugo natural",
                    21 => "Smoothie de frutas",
                    22 => "Chocolate",
                    23 => "Tarta de manzana",
                    24 => "Pescado al horno",
                    25 => "Pasta boloñesa",
                    26 => "Lomo a la pimienta",
                    27 => "Ensalada de frutas",
                    28 => "Sándwich de jamón y queso",
                    29 => "Huevos revueltos",
                    30 => "Cerveza artesanal",
                    31 => "Vino tinto",
                    _ => "Desconocido"
                }
            }).ToArray();

        modelBuilder.Entity<Gusto>().HasData(gustos);

        // --- GUSTO ↔ TAG ---
        var gt = new List<object>();
        void GT(int i, string nombreTag)
        {
            var tag = tags.First(t => t.Nombre == nombreTag);
            gt.Add(new { GustosId = gustos[i - 1].Id, TagsId = tag.Id });
        }

        GT(1, "Gluten"); GT(1, "Lácteo"); GT(1, "Sal");
        GT(2, "Pescado"); GT(2, "Sal");
        GT(3, "Mariscos"); GT(3, "Sal");
        GT(4, "Carne roja"); GT(4, "Frito"); GT(4, "Harina");
        GT(5, "Carne roja"); GT(5, "Picante");
        GT(6, "Vegetal"); GT(6, "Lácteo"); GT(6, "Sal");
        GT(7, "Gluten"); GT(7, "Sal"); GT(7, "Picante");
        GT(8, "Gluten"); GT(8, "Carne roja"); GT(8, "Frito");
        GT(9, "Pescado"); GT(9, "Mariscos"); GT(9, "Vegetal");
        GT(10, "Lácteo"); GT(10, "Azúcar");
        GT(11, "Carne roja"); GT(11, "Gluten"); GT(11, "Frito");
        GT(12, "Frito"); GT(12, "Sal");
        GT(13, "Carne blanca"); GT(13, "Vegetal");
        GT(14, "Carne roja"); GT(14, "Picante"); GT(14, "Grasa");
        GT(15, "Vegetal");
        GT(16, "Carne roja"); GT(16, "Sal"); GT(16, "Grasa");
        GT(17, "Vegetal"); GT(17, "Sal");
        GT(18, "Harina"); GT(18, "Huevos"); GT(18, "Azúcar");
        GT(19, "Cafeína"); GT(19, "Lácteo");
        GT(20, "Fruta");
        GT(21, "Fruta"); GT(21, "Azúcar");
        GT(22, "Azúcar"); GT(22, "Grasa");
        GT(23, "Fruta"); GT(23, "Harina"); GT(23, "Azúcar");
        GT(24, "Pescado"); GT(24, "Vegetal");
        GT(25, "Gluten"); GT(25, "Carne roja");
        GT(26, "Carne roja"); GT(26, "Sal");
        GT(27, "Fruta");
        GT(28, "Gluten"); GT(28, "Lácteo"); GT(28, "Procesado");
        GT(29, "Huevos");
        GT(30, "Alcohol");
        GT(31, "Alcohol");

        modelBuilder.Entity("GustoTags").HasData(gt);

        // --- RESTRICCIONES ---
        var restricciones = Enumerable.Range(1, 12)
            .Select(i => new Restriccion
            {
                Id = Guid.Parse($"33333333-0001-0001-0001-{i:D12}"),
                Nombre = i switch
                {
                    1 => "Sin gluten",
                    2 => "Sin lactosa",
                    3 => "Sin azúcar",
                    4 => "Sin sal",
                    5 => "Sin mariscos",
                    6 => "Sin carne roja",
                    7 => "Sin frito",
                    8 => "Sin picante",
                    9 => "Sin cafeína",
                    10 => "Sin alcohol",
                    11 => "Sin soja",
                    12 => "Sin frutos secos",
                    _ => "Restriccion desconocida"
                }
            }).ToArray();
        modelBuilder.Entity<Restriccion>().HasData(restricciones);

        var rt = new List<object>();
        void RT(int i, string nombreTag)
        {
            var tag = tags.First(t => t.Nombre == nombreTag);
            rt.Add(new { RestriccionesId = restricciones[i - 1].Id, TagsId = tag.Id });
        }

        RT(1, "Gluten"); RT(2, "Lácteo"); RT(3, "Azúcar"); RT(4, "Sal");
        RT(5, "Mariscos"); RT(6, "Carne roja"); RT(7, "Frito");
        RT(8, "Picante"); RT(9, "Cafeína"); RT(10, "Alcohol");
        RT(11, "Soja"); RT(12, "Frutos secos");
        modelBuilder.Entity("RestriccionTags").HasData(rt);

        // --- CONDICIONES MÉDICAS ---
        var condiciones = Enumerable.Range(1, 12)
            .Select(i => new CondicionMedica
            {
                Id = Guid.Parse($"44444444-0001-0001-0001-{i:D12}"),
                Nombre = i switch
                {
                    1 => "Diabetes",
                    2 => "Hipertensión",
                    3 => "Colesterol alto",
                    4 => "Gastritis",
                    5 => "Enfermedad celíaca",
                    6 => "Intolerancia a la lactosa",
                    7 => "Alergia a mariscos",
                    8 => "Alergia a frutos secos",
                    9 => "Alergia al huevo",
                    10 => "Síndrome del intestino irritable",
                    11 => "Gota",
                    12 => "Ansiedad (sensibilidad a cafeína)",
                    _ => "Condición desconocida"
                }
            }).ToArray();
        modelBuilder.Entity<CondicionMedica>().HasData(condiciones);

        var ct = new List<object>();
        void CT(int i, string nombreTag)
        {
            var tag = tags.First(t => t.Nombre == nombreTag);
            ct.Add(new { CondicionesMedicasId = condiciones[i - 1].Id, TagsId = tag.Id });
        }

        CT(1, "Azúcar"); CT(2, "Sal"); CT(3, "Grasa"); CT(4, "Picante");
        CT(5, "Gluten"); CT(6, "Lácteo"); CT(7, "Mariscos"); CT(8, "Frutos secos");
        CT(9, "Huevos"); CT(10, "Frito"); CT(11, "Carne roja"); CT(12, "Cafeína");
        modelBuilder.Entity("CondicionMedicaTags").HasData(ct);
    }
}

