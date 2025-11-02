namespace GustosApp.Infraestructure;

using GustosApp.Application.DTO;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;


using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Tokenizers.HuggingFace.Decoders;


public class GustosDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Gusto> Gustos { get; set; }
    public DbSet<Restriccion> Restricciones { get; set; }
    public DbSet<CondicionMedica> CondicionesMedicas { get; set; }
    public DbSet<Grupo> Grupos { get; set; }
    public DbSet<MiembroGrupo> MiembrosGrupos { get; set; }
    public DbSet<InvitacionGrupo> InvitacionesGrupos { get; set; }
    public DbSet<SolicitudAmistad> SolicitudesAmistad { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }


    public DbSet<RestauranteEspecialidad> RestauranteEspecialidades { get; set; }
    public DbSet<Restaurante> Restaurantes { get; set; }

    public DbSet<ReviewRestaurante> ReviewsRestaurantes { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<RestaurantePlato> RestaurantePlatos { get; set; }

    public DbSet<GrupoGusto> GrupoGustos { get; set; }

    public DbSet<Notificacion> Notificaciones { get; set; }

    public DbSet<RestauranteImagen> RestauranteImagenes { get; set; }
    public DbSet<RestauranteMenu> RestauranteMenus { get; set; }

    public DbSet<UsuarioRestauranteVisitado> UsuarioRestauranteVisitados { get; set; }

    public GustosDbContext(DbContextOptions<GustosDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new GustosApp.Infraestructure.Configurations.UsuarioRestauranteVisitadoConfiguration());

        modelBuilder.ApplyConfiguration(new GustosApp.Infraestructure.Configurations.RestauranteImagenConfiguration());
        modelBuilder.ApplyConfiguration(new GustosApp.Infraestructure.Configurations.RestauranteMenuConfiguration());

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

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Restricciones)
            .WithMany(r => r.Usuarios)
            .UsingEntity(j => j.ToTable("UsuarioRestricciones"));

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

        modelBuilder.Entity<SolicitudAmistad>()
            .HasOne(s => s.Remitente)
            .WithMany()
            .HasForeignKey("RemitenteId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitudAmistad>()
            .HasOne(s => s.Destinatario)
            .WithMany()
            .HasForeignKey("DestinatarioId")
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitudAmistad>()
            .Property(s => s.Estado)
            .HasConversion<int>();

        // 1. Configuración de la clave compuesta para la tabla GrupoGustos
        modelBuilder.Entity<GrupoGusto>()
            .HasKey(gg => gg.Id);

        // 2. Relación de GrupoGustos con Grupo
        modelBuilder.Entity<GrupoGusto>()
            .HasOne(gg => gg.Grupo)
            .WithMany(g => g.Gustos) // Asegúrate de añadir la colección 'Gustos' a la clase Grupo
            .HasForeignKey(gg => gg.GrupoId)
            .OnDelete(DeleteBehavior.Cascade); // Si el grupo se elimina, se eliminan sus gustos asociados

        // 3. Relación de GrupoGustos con Gusto
        modelBuilder.Entity<GrupoGusto>()
            .HasOne(gg => gg.Gusto)
            .WithMany(g => g.GruposRelacionados) // Asegúrate de añadir la colección 'GruposRelacionados' a la clase Gusto
            .HasForeignKey(gg => gg.GustoId)
            .OnDelete(DeleteBehavior.Restrict); // No eliminar un Gusto si está siendo usado por grupos

        // Chat messages
        modelBuilder.Entity<ChatMessage>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<ChatMessage>()
            .Property(c => c.Mensaje)
            .IsRequired();

        modelBuilder.Entity<Restaurante>()
            .HasMany(r => r.GustosQueSirve)
            .WithMany(g => g.restaurantes)
            .UsingEntity(j => j.ToTable("RestauranteGustos"));

        modelBuilder.Entity<Restaurante>()
            .HasMany(r => r.RestriccionesQueRespeta)
            .WithMany(p => p.Restaurantes)
            .UsingEntity(j => j.ToTable("RestauranteRestricciones"));

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.ToTable("Notificaciones");

            entity.HasKey(n => n.Id);

            entity.Property(n => n.Titulo)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(n => n.Mensaje)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(n => n.Tipo)
                .IsRequired();

            entity.Property(n => n.UsuarioDestinoId)
                .IsRequired();


            entity.HasOne(n => n.UsuarioDestino)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(n => n.UsuarioDestinoId)
                .OnDelete(DeleteBehavior.Cascade);


            entity.HasOne(n => n.Invitacion)
                .WithOne(i => i.Notificacion)
                .HasForeignKey<Notificacion>(n => n.InvitacionId)
                .OnDelete(DeleteBehavior.SetNull);
        });



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
        var gustos = Enumerable.Range(1, 64)
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
                    32 => "Ensalada Caprese",
                    33 => "Tarta de Verduras",
                    34 => "Omelette de vegetales",
                    35 => "Pizza Margarita",
                    36 => "Milanesa de berenjena",
                    37 => "Ñoquis con salsa de tomate",
                    38 => "Ravioles de ricota y espinaca",
                    39 => "Fideos con pesto",
                    40 => "Panqueques de avena con frutas",
                    41 => "Empanadas de humita",
                    42 => "Lasaña vegetariana",
                    43 => "Arroz primavera",
                    44 => "Polenta con salsa de tomate",
                    45 => "Sopa de calabaza",
                    46 => "Tortilla de papas",
                    47 => "Quesadillas de vegetales",
                    48 => "Bruschettas con tomate y albahaca",
                    49 => "Pastel de papas vegetariano",
                    50 => "Pizza cuatro quesos",
                    51 => "Ensalada de quinoa con vegetales",
                    52 => "Curry de vegetales con arroz",
                    53 => "Hamburguesa de lentejas",
                    54 => "Sopa crema de zapallo con leche vegetal",
                    55 => "Arroz frito con tofu",
                    56 => "Guiso de lentejas vegano",
                    57 => "Pan integral con palta y tomate",
                    58 => "Panqueques de banana sin huevo",
                    59 => "Wrap de falafel con hummus",
                    60 => "Brownie vegano(con harina integral y aceite de coco",
                    61 => "Tarta vegana de calabaza",
                    62 => "Empanadas veganas",
                    63 => "Tacos veganos",
                    64 => "Fideos de arroz con verduras salteadas",
                    _ => "Desconocido"
                },
                ImagenUrl = i switch
                {
                    1 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pizza.jpg?alt=media&token=1e4e7fea-31d3-4e04-ae50-1ebe29fd16f2",
                    2 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sushi.jpg?alt=media&token=9dfd9b64-8455-4206-a5ec-090c935e86e7",
                    3 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/paella.jpg?alt=media&token=5cfd79d4-7e92-452e-a7c4-899b374d3ea8",
                    4 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/milanesa-con-papas-fritas.jpg?alt=media&token=d2ca59bc-6360-4378-919a-886b0c0e93e0",
                    5 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tacos.jpg?alt=media&token=431ae163-15e9-41d0-8fa6-6f79e9862150",
                    6 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada-cesar.jpg?alt=media&token=a6b5eaf0-be77-4716-8b11-18f3774f004f",
                    7 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ramen.jpg?alt=media&token=886fdc48-3d43-46fd-9911-48b1966da347",
                    8 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/empanadas.png?alt=media&token=7438d05a-c0be-4da0-aea6-b6ab26f7f621",
                    9 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ceviche.jpg?alt=media&token=ad28a0df-4bc0-4aa8-ae02-610526ac1152",
                    10 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/helado.jpg?alt=media&token=01be542d-9cc4-47f3-a27f-ae3a1b80d306",
                    11 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Hamburguesa.jpg?alt=media&token=a0fd669b-ade3-427c-b428-c743338885c8",
                    12 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/papas-fritas.jpg?alt=media&token=5b18bf54-256e-4b36-adc3-e438fa3d374c",
                    13 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pollo-grill.jpg?alt=media&token=d60229b3-5e8a-4de6-9ed7-4a9622a2f3e1",
                    14 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/kebab.jpg?alt=media&token=0acd13ee-654c-4748-bbe4-695a06053f75",
                    15 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada-verde.jpg?alt=media&token=0bb027c8-de8d-4ac4-99db-ee80fc7d0f1c",
                    16 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/asado.jpg?alt=media&token=254fbe63-39ba-4529-87bb-556381370c9a",
                    17 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sopa-verduras.jpg?alt=media&token=9858f540-0cb8-4759-a7d5-a6743144863e",
                    18 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/panquques.jpg?alt=media&token=2b203013-cbcf-40f3-b266-d629466cd0b2",
                    19 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/cafe-con-leche.jpg?alt=media&token=1f3cde3c-e3b9-4ed0-a690-d7e4d6875447",
                    20 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/jugos-naturales.jpg?alt=media&token=b8afeb02-882e-4a3f-8445-14949e5871dd",
                    21 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Smoothie-frutas.jpg?alt=media&token=17ddc1f6-d60c-40d4-8368-f1f003ddd62b",
                    22 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/chocolates.jpg?alt=media&token=5f33a2af-cee7-4768-8b47-dda973dd9c4e",
                    23 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tarta-manzana.jpg?alt=media&token=c34fa589-0c37-44cb-891e-2e8aaebdb215",
                    24 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Pescado-al-horn.png?alt=media&token=953296de-c3be-47a4-826e-31ad29cbae22",
                    25 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pasta-bolognesa.jpg?alt=media&token=7893a48f-f693-4a2f-a574-81336f91e62c",
                    26 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/lomo_a_la_pimienta.png?alt=media&token=4d8495a4-181b-4d50-a9c8-e95010bfb100",
                    27 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada_frutas.jpg?alt=media&token=a13662fa-2ac4-40cd-b1f0-347634a991e9",
                    28 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sandwich-de-huevo-con-jamon-y-queso.jpg?alt=media&token=8ac9b05d-c316-4a84-a099-9bca4f2d6a9a",
                    29 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/huevos-revueltos-desayuno.jpeg?alt=media&token=0f21b637-b499-427c-bdb0-f0a841a76a9b",
                    30 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tipos-de-cerveza.jpg?alt=media&token=1cfa9e77-b663-421a-b649-d52a1ba751d2",
                    31 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/vino_artesanal.jpg?alt=media&token=fd22ec00-7739-4776-b488-63e46c2937c5",
                    _ => null
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
        GT(32, "Lácteo"); GT(32, "Vegetal");
        GT(33, "Vegetal"); GT(33, "Harina");
        GT(34, "Huevos"); GT(34, "Vegetal");
        GT(35, "Gluten"); GT(35, "Lácteo"); GT(35, "Harina");
        GT(36, "Harina"); GT(36, "Vegetal"); GT(36, "Lácteo");
        GT(37, "Harina"); GT(37, "Sal");
        GT(38, "Gluten"); GT(38, "Lácteo"); GT(38, "Harina");
        GT(39, "Gluten"); GT(39, "Harina"); GT(39, "Grasa");
        GT(40, "Harina"); GT(40, "Fruta"); GT(40, "Huevos");
        GT(41, "Harina"); GT(41, "Vegetal"); GT(41, "Lácteo");
        GT(42, "Harina"); GT(42, "Lácteo"); GT(42, "Vegetal");
        GT(43, "Vegetal");
        GT(44, "Sal"); GT(44, "Vegetal");
        GT(45, "Vegetal");
        GT(46, "Huevos"); GT(46, "Frito"); GT(46, "Vegetal");
        GT(47, "Harina"); GT(47, "Lácteo"); GT(47, "Vegetal");
        GT(48, "Harina"); GT(48, "Vegetal");
        GT(49, "Vegetal"); GT(49, "Lácteo");
        GT(50, "Gluten"); GT(50, "Lácteo"); GT(50, "Harina"); GT(50, "Grasa");
        GT(51, "Vegetal");
        GT(52, "Vegetal"); GT(52, "Picante");
        GT(53, "Harina"); GT(53, "Vegetal");
        GT(54, "Vegetal");
        GT(55, "Vegetal"); GT(55, "Frito"); GT(55, "Soja");
        GT(56, "Vegetal");
        GT(57, "Harina"); GT(57, "Vegetal");
        GT(58, "Harina"); GT(58, "Fruta");
        GT(59, "Harina"); GT(59, "Vegetal"); GT(59, "Soja");
        GT(60, "Harina"); GT(60, "Grasa"); GT(60, "Azúcar");
        GT(61, "Vegetal"); GT(61, "Harina");
        GT(62, "Harina"); GT(62, "Vegetal");
        GT(63, "Harina"); GT(63, "Vegetal");
        GT(64, "Vegetal");

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
        var condiciones = Enumerable.Range(1, 14)
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
                    13 => "Vegetariano",
                    14 => "Vegano",
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
        CT(13, "Carne roja"); CT(13, "Carne blanca"); CT(13, "Pescado");
        CT(14, "Carne roja"); CT(14, "Carne blanca"); CT(14, "Pescado"); CT(14, "Huevos"); CT(14, "Lácteo");
        modelBuilder.Entity("CondicionMedicaTags").HasData(ct);
    }
}

