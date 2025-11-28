namespace GustosApp.Infraestructure;

using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;


using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Tokenizers.HuggingFace.Decoders;
using GustosApp.Domain.Model.@enum;
using GustosApp.Application.Interfaces;

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
    public DbSet<ChatMensaje> ChatMessages { get; set; }


    public DbSet<RestauranteEspecialidad> RestauranteEspecialidades { get; set; }
    public DbSet<Restaurante> Restaurantes { get; set; }

    public DbSet<RestauranteEstadisticas> RestauranteEstadisticas { get; set; }

    public DbSet<SolicitudRestaurante> SolicitudesRestaurantes { get; set; }

    //public DbSet<ReseñaRestaurante> ReseñasRestaurantes { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<RestaurantePlato> RestaurantePlatos { get; set; }

    public DbSet<GrupoGusto> GrupoGustos { get; set; }

    public DbSet<Notificacion> Notificaciones { get; set; }

    public DbSet<RestauranteImagen> RestauranteImagenes { get; set; }
    public DbSet<RestauranteMenu> RestauranteMenus { get; set; }
    public DbSet<SolicitudRestauranteImagen> SolicitudRestauranteImagenes { get; set; }


    public DbSet<UsuarioRestauranteVisitado> UsuarioRestauranteVisitados { get; set; }

    //public DbSet<Valoracion> Valoraciones { get; set; }

    public DbSet<OpinionRestaurante> OpinionesRestaurantes { get; set; }
    public DbSet<OpinionFoto> OpinionesFotos { get; set; }

    
    public DbSet<VotacionGrupo> Votaciones { get; set; }
    public DbSet<VotoRestaurante> Votos { get; set; }

    public DbSet<OpinionRestaurante> OpinionesRestaurante { get; set; }
    public DbSet<UsuarioRestauranteFavorito> UsuarioRestauranteFavoritos { get; set; }


    public GustosDbContext(DbContextOptions<GustosDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new Configurations.UsuarioRestauranteVisitadoConfiguration());

        modelBuilder.ApplyConfiguration(new Configurations.RestauranteImagenConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.RestauranteMenuConfiguration());
        modelBuilder.ApplyConfiguration(new GustosApp.Infraestructure.Configurations.RestauranteEstadisticasConfiguration());

        modelBuilder.ApplyConfiguration(new GustosApp.Infraestructure.Configurations.VotacionGrupoConfiguration());
        modelBuilder.ApplyConfiguration(new GustosApp.Infraestructure.Configurations.VotoRestauranteConfiguration());



        modelBuilder.Entity<SolicitudRestaurante>()
    .Property(s => s.Latitud)
    .HasColumnType("decimal(10,7)");

        modelBuilder.Entity<SolicitudRestaurante>()
            .Property(s => s.Longitud)
            .HasColumnType("decimal(10,7)");

        modelBuilder.Entity<SolicitudRestaurante>()
    .HasIndex(s => s.Estado);

        modelBuilder.Entity<SolicitudRestaurante>()
            .Property(s => s.Nombre)
            .HasMaxLength(150)
            .IsRequired();


        modelBuilder.Entity<SolicitudRestaurante>()
       .HasOne(s => s.Usuario)
       .WithMany(u => u.SolicitudesRestaurantes)
       .HasForeignKey(s => s.UsuarioId)
       .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitudRestaurante>()
            .HasMany(s => s.Gustos)
            .WithMany()
            .UsingEntity(j => j.ToTable("SolicitudRestaurante_Gustos"));

        modelBuilder.Entity<SolicitudRestaurante>()
            .HasMany(s => s.Restricciones)
            .WithMany()
            .UsingEntity(j => j.ToTable("SolicitudRestaurante_Restricciones"));


        modelBuilder.Entity<SolicitudRestauranteImagen>()
          .HasOne(i => i.Solicitud)
           .WithMany(s => s.Imagenes)
           .HasForeignKey(i => i.SolicitudId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Restaurante>()
        .Ignore(r => r.Score);

        modelBuilder.Entity<Restaurante>()
         .HasOne(r => r.Menu)
         .WithOne(m => m.Restaurante)
         .HasForeignKey<RestauranteMenu>(m => m.RestauranteId)
         .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Restaurante>()
                .HasMany(r => r.Reviews)
                .WithOne()
                .HasForeignKey(r => r.RestauranteId)

                .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Restaurante>()
            .HasOne(r => r.Dueno)
            .WithMany(u => u.Restaurantes)
            .HasForeignKey(r => r.DuenoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Restaurante>()
             .HasMany(r => r.GustosQueSirve)
             .WithMany(g => g.restaurantes)
             .UsingEntity(j => j.ToTable("RestauranteGustos"));

        modelBuilder.Entity<Restaurante>()
            .HasMany(r => r.RestriccionesQueRespeta)
            .WithMany(p => p.Restaurantes)
            .UsingEntity(j => j.ToTable("RestauranteRestricciones"));


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
        modelBuilder.Entity<SolicitudAmistad>(entity =>
        {
            entity.Property(s => s.Estado)
                .HasConversion<int>();

            entity.HasOne(s => s.Remitente)
                .WithMany()
                .HasForeignKey(s => s.RemitenteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Destinatario)
                .WithMany()
                .HasForeignKey(s => s.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });



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

        // 4. Relación de GrupoGusto con MiembroGrupo
        modelBuilder.Entity<GrupoGusto>()
            .HasOne(gg => gg.Miembro)
            .WithMany(m => m.GustosSeleccionados)
            .HasForeignKey(gg => gg.MiembroId)
            .OnDelete(DeleteBehavior.Restrict); 

        // Chat messages
        modelBuilder.Entity<ChatMensaje>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<ChatMensaje>()
            .Property(c => c.Mensaje)
            .IsRequired();

        

        modelBuilder.Entity<UsuarioRestauranteFavorito>()
            .HasIndex(f => new { f.UsuarioId, f.RestauranteId })
            .IsUnique(); // Evita duplicados

        modelBuilder.Entity<UsuarioRestauranteFavorito>()
            .HasOne(f => f.Usuario)
            .WithMany(u => u.RestaurantesFavoritos)
            .HasForeignKey(f => f.UsuarioId);

        modelBuilder.Entity<UsuarioRestauranteFavorito>()
            .HasOne(f => f.Restaurante)
            .WithMany()
            .HasForeignKey(f => f.RestauranteId);

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

        modelBuilder.Entity<OpinionRestaurante>()
            .HasOne(o => o.Restaurante)
            .WithMany(r => r.Reviews)
            .HasForeignKey(o => o.RestauranteId)
            .OnDelete(DeleteBehavior.Cascade);

      
        modelBuilder.Entity<OpinionRestaurante>()
            .HasOne(o => o.Usuario)
            .WithMany()
            .HasForeignKey(o => o.UsuarioId)
            .OnDelete(DeleteBehavior.NoAction);
        // (NoAction evita eliminar opiniones si se borra un usuario; se podrían conservar como "opiniones antiguas")

      
        modelBuilder.Entity<OpinionFoto>()
            .HasOne(f => f.Opinion)
            .WithMany(o => o.Fotos)
            .HasForeignKey(f => f.OpinionRestauranteId)
            .OnDelete(DeleteBehavior.Cascade);


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
                    1 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpizza%20(1)%20(1).jpg?alt=media&token=acae5ec9-56ff-4b1e-9f52-c6e3aa1c0465",
                    2 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fsushi%20(1).jpg?alt=media&token=699bb1f7-9ca8-467c-a07c-c25b85e155dd",
                    3 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpaella%20(1)%20(1).jpg?alt=media&token=8d28a0aa-4e53-4552-ad79-4007f27ea6ef",
                    4 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/milanesa-con-papas-fritas.jpg?alt=media&token=d2ca59bc-6360-4378-919a-886b0c0e93e0",
                    5 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tacos.jpg?alt=media&token=431ae163-15e9-41d0-8fa6-6f79e9862150",
                    6 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fensalada-cesar%20(1).jpg?alt=media&token=c478c0b8-7b06-40d3-b3b6-b4d8a1c7af45",
                    7 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Framen%20(1).jpg?alt=media&token=f50e95b0-8f37-499a-803e-2f9ce530d68a",
                    8 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fempanadas.jpg?alt=media&token=07a8e917-a280-4345-9717-fe0b707dc8e7",
                    9 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ceviche.jpg?alt=media&token=ad28a0df-4bc0-4aa8-ae02-610526ac1152",
                    10 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fhelado%20(1).jpg?alt=media&token=eef033a6-344a-41af-904a-7ff1116671c3",
                    11 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Hamburguesa.jpg?alt=media&token=a0fd669b-ade3-427c-b428-c743338885c8",
                    12 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/papas-fritas.jpg?alt=media&token=5b18bf54-256e-4b36-adc3-e438fa3d374c",
                    13 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpollo-grill%20(1)%20(1).jpg?alt=media&token=22febf47-6138-4f88-a0a5-d131ef67f44c",
                    14 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/kebab.jpg?alt=media&token=0acd13ee-654c-4748-bbe4-695a06053f75",
                    15 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada-verde.jpg?alt=media&token=0bb027c8-de8d-4ac4-99db-ee80fc7d0f1c",
                    16 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fasado%20(1).jpg?alt=media&token=86e67902-4e82-4d36-87df-b6f4973e9b61",
                    17 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fsopa-verduras%20(1).jpg?alt=media&token=a9f74b05-2726-4a12-89fc-51e9ab99f895",
                    18 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/panquques.jpg?alt=media&token=2b203013-cbcf-40f3-b266-d629466cd0b2",
                    19 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/cafe-con-leche.jpg?alt=media&token=1f3cde3c-e3b9-4ed0-a690-d7e4d6875447",
                    20 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/jugos-naturales.jpg?alt=media&token=b8afeb02-882e-4a3f-8445-14949e5871dd",
                    21 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Smoothie-frutas.jpg?alt=media&token=17ddc1f6-d60c-40d4-8368-f1f003ddd62b",
                    22 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fchocolates%20(1).jpg?alt=media&token=52113b04-41a3-4e40-85bf-c6887227528c",
                    23 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Ftarta-manzana%20(1).jpg?alt=media&token=a16c689c-6cbb-4e77-a9f3-5bf448dc6450",
                    24 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2FPescado-al-horn.jpg?alt=media&token=dd7a4dee-6229-4766-83e1-2647ab36b64c",
                    25 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pasta-bolognesa.jpg?alt=media&token=7893a48f-f693-4a2f-a574-81336f91e62c",
                    26 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Flomo_a_la_pimienta.jpg?alt=media&token=1b1b8e30-083a-4ad2-854b-7903a4f750c9",
                    27 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada_frutas.jpg?alt=media&token=a13662fa-2ac4-40cd-b1f0-347634a991e9",
                    28 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sandwich-de-huevo-con-jamon-y-queso.jpg?alt=media&token=8ac9b05d-c316-4a84-a099-9bca4f2d6a9a",
                    29 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fhuevos-revueltos-desayuno.jpg?alt=media&token=c08b7124-4f73-4c68-b85e-7f8949b65688",
                    30 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tipos-de-cerveza.jpg?alt=media&token=1cfa9e77-b663-421a-b649-d52a1ba751d2",
                    31 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/vino_artesanal.jpg?alt=media&token=fd22ec00-7739-4776-b488-63e46c2937c5",
                    32 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fensalada-caprese.jpg?alt=media&token=6bc93fe3-acc5-46b4-be63-c1de7b621854", // Ensalada Caprese
                    33 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftarta-verduras.jpg?alt=media&token=65092fe1-acfa-4c5a-84c4-ebcf00ad6068", // Tarta de Verduras
                    34 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fomelette-de-verduras.jpg?alt=media&token=25b514e3-5f87-43a0-99b0-83b0b9af243f", // Omelette de vegetales
                    35 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fpizza-margarita.jpg?alt=media&token=0ae93afc-274e-4a4b-879a-8d288cac8d3a", // Pizza Margarita
                    36 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FMILANESAS-DE-BERENJENA-CON-PURE-DE-ARVEJAS-05%20(1).jpg?alt=media&token=f21fab62-9138-450d-be46-faf0acf43ef1", // Milanesa de berenjena
                    37 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fnoquis-caseros-con-salsa-de-tomate-1200x900.jpg?alt=media&token=35667f32-5775-41b6-9d29-aed9acf2a64b", // Ñoquis con salsa de tomate
                    38 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FRavioles-ricota-espinaca%20(1).jpg?alt=media&token=89d4e8ba-7db8-40d5-8d1a-09cf16216af8", // Ravioles de ricota y espinaca
                    39 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ffideos_con_pesto.jpg?alt=media&token=47cbf2d7-53f2-4710-88a8-ef8b43fb3dfc", // Fideos con pesto
                    40 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPanquequavenarutas.jpg?alt=media&token=0d4dcf14-a9e3-48c1-a6ff-06118c259555", // Panqueques de avena con frutas
                    41 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fempanda_humita.jpg?alt=media&token=862cd874-b8b2-4793-93f5-284268db5f6d", // Empanadas de humita
                    42 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FLasa%C3%B1a-vegetariana.jpg?alt=media&token=2ad7e9ef-a341-43d3-b135-9b22b2f9d0ba", // Lasaña vegetariana
                    43 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FArrozprimavera.jpg?alt=media&token=3872ad34-aec3-4610-af74-88a5ed01894f", // Arroz primavera
                    44 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPolenta-salsa-tomate.jpg?alt=media&token=9869796e-f39d-4536-afe4-b79131bdcaa9", // Polenta con salsa de tomate
                    45 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fsopa-de-calabaza_web.jpg?alt=media&token=06b7d888-fea8-4f88-8447-c8c4494018e9", // Sopa de calabaza
                    46 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftortilla-de-papas-con-EPMZH233TBBEBETIIAYLJQ57JU.jpg?alt=media&token=42079a0c-02c9-4de4-8e9e-eff9d9c59d9f", // Tortilla de papas
                    47 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FQuesadillas-vegetales.jpg?alt=media&token=2f5fcc35-826f-4134-a50f-a7a81f2e20d2", // Quesadillas de vegetales
                    48 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FBruschetta-tomate-albahaca.jpg?alt=media&token=3dc0476c-f168-40f1-afa1-d557e2ab4f90", // Bruschettas con tomate y albahaca
                    49 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPastel-papas-vegetariano.jpg?alt=media&token=81f3f01a-f6cc-4489-91ca-d191d30a5567", // Pastel de papas vegetariano
                    50 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPizza-cuatro-quesos%20(1).jpg?alt=media&token=d92594ca-b9f7-42ac-b2b1-ceb16f269740", // Pizza cuatro quesos
                    51 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FEnsalada-de-quinoa-con-verduras.jpg?alt=media&token=aec93b8a-c0ae-431e-b006-8db27840fdc5", // Ensalada de quinoa con vegetales
                    52 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Farroz-al-curry-con-2HA57GEWZJFM5CD27MHLFJ6ZMU.jpg?alt=media&token=65998321-d8c5-4dea-ba17-dcb311f2cf77", // Curry de vegetales con arroz
                    53 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fhamburguesa-lentejas-7.jpg?alt=media&token=a418381f-009f-47dc-ac6d-1713c94df7bf", // Hamburguesa de lentejas
                    54 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FSopa-crema-zapallo-leche-vegetal.jpg?alt=media&token=fa58ead1-8dbd-4121-a5e0-5f9428713a57", // Sopa crema de zapallo con leche vegetal
                    55 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Farroz_frito_con_tofu_62074_orig.jpg?alt=media&token=1e451e0b-8eef-4e0d-a761-c9364e6ec4c9", // Arroz frito con tofu
                    56 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FGuiso-lentejas-vegano.jpg?alt=media&token=3ef78d85-c7e4-4fb6-b1cc-8c00b183dea0", // Guiso de lentejas vegano
                    57 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fpan_con_palta_tomate.jpg?alt=media&token=2c63b3e3-4092-46f2-b852-4e382a86f452", // Pan integral con palta y tomate
                    58 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPanqueques-banana-sin-huevo.jpg?alt=media&token=ccea698a-3c15-41d0-82dd-5080f3a91210", // Panqueques de banana sin huevo
                    59 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FWrap-falafel-hummus.jpg?alt=media&token=26a4a908-3460-4eb2-b372-077acf14fe1b", // Wrap de falafel con hummus
                    60 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FBrownievegano(con%20harina%20integra%20aceite%20de%20coco.jpg?alt=media&token=96a9ce83-43bf-4d92-9fbb-d6c9bdd0701c", // Brownie vegano (con harina integral y aceite de coco)
                    61 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftarta-de-calabaza-vegana-pumpkin-pie-saludable.jpg?alt=media&token=21f2a9f5-33d6-4968-9963-21615ac588a6", // Tarta vegana de calabaza
                    62 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fempanadillas2-1024x683.jpg?alt=media&token=20a2c707-ca29-4a1e-a194-9be0fa3424dd", // Empanadas veganas
                    63 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftacos_veganos.jpg?alt=media&token=9437ffc6-8a1f-4c93-89f3-33f0e4d35ebf", // Tacos veganos
                    64 => "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FFideos-arrozrerdurassalteadas.jpg?alt=media&token=a17821fc-210e-4078-856a-d892c75b5a93", // Fideos de arroz con verduras salteadas

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

