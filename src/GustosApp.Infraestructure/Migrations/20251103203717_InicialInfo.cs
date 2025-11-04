using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InicialInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CondicionesMedicas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionesMedicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gustos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gustos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropietarioUid = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    NombreNormalizado = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Latitud = table.Column<double>(type: "float", nullable: false),
                    Longitud = table.Column<double>(type: "float", nullable: false),
                    HorariosJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlaceId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    CantidadResenas = table.Column<int>(type: "int", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WebUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    EmbeddingVector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false, defaultValue: "restaurant"),
                    TypesJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    ImagenUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Valoracion = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restricciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restricciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EsPrivado = table.Column<bool>(type: "bit", nullable: false),
                    FirebaseUid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FotoPerfilUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Plan = table.Column<int>(type: "int", nullable: false),
                    PasoActual = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReseñasRestaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReseñasRestaurantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReseñasRestaurantes_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestauranteEspecialidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestauranteEspecialidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestauranteEspecialidades_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestauranteGustos",
                columns: table => new
                {
                    GustosQueSirveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    restaurantesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestauranteGustos", x => new { x.GustosQueSirveId, x.restaurantesId });
                    table.ForeignKey(
                        name: "FK_RestauranteGustos_Gustos_GustosQueSirveId",
                        column: x => x.GustosQueSirveId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestauranteGustos_Restaurantes_restaurantesId",
                        column: x => x.restaurantesId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestauranteImagenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: true),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestauranteImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestauranteImagenes_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestauranteMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Json = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    FechaActualizacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestauranteMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestauranteMenus_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantePlatos",
                columns: table => new
                {
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Plato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantePlatos", x => new { x.RestauranteId, x.Plato });
                    table.ForeignKey(
                        name: "FK_RestaurantePlatos_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestauranteRestricciones",
                columns: table => new
                {
                    RestaurantesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestriccionesQueRespetaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestauranteRestricciones", x => new { x.RestaurantesId, x.RestriccionesQueRespetaId });
                    table.ForeignKey(
                        name: "FK_RestauranteRestricciones_Restaurantes_RestaurantesId",
                        column: x => x.RestaurantesId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestauranteRestricciones_Restricciones_RestriccionesQueRespetaId",
                        column: x => x.RestriccionesQueRespetaId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CondicionMedicaTags",
                columns: table => new
                {
                    CondicionesMedicasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionMedicaTags", x => new { x.CondicionesMedicasId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_CondicionMedicaTags_CondicionesMedicas_CondicionesMedicasId",
                        column: x => x.CondicionesMedicasId,
                        principalTable: "CondicionesMedicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CondicionMedicaTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GustoTags",
                columns: table => new
                {
                    GustosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GustoTags", x => new { x.GustosId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_GustoTags_Gustos_GustosId",
                        column: x => x.GustosId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GustoTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestriccionTags",
                columns: table => new
                {
                    RestriccionesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionTags", x => new { x.RestriccionesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_RestriccionTags_Restricciones_RestriccionesId",
                        column: x => x.RestriccionesId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestriccionTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdministradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CodigoInvitacion = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaExpiracionCodigo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grupos_Usuarios_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAmistad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemitenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinatarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAmistad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAmistad_Usuarios_DestinatarioId",
                        column: x => x.DestinatarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesAmistad_Usuarios_RemitenteId",
                        column: x => x.RemitenteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioCondicionesMedicas",
                columns: table => new
                {
                    CondicionesMedicasId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuariosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCondicionesMedicas", x => new { x.CondicionesMedicasId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_UsuarioCondicionesMedicas_CondicionesMedicas_CondicionesMedicasId",
                        column: x => x.CondicionesMedicasId,
                        principalTable: "CondicionesMedicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCondicionesMedicas_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioGustos",
                columns: table => new
                {
                    GustosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuariosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioGustos", x => new { x.GustosId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_UsuarioGustos_Gustos_GustosId",
                        column: x => x.GustosId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioGustos_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRestauranteVisitados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlaceId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Latitud = table.Column<double>(type: "float", nullable: false),
                    Longitud = table.Column<double>(type: "float", nullable: false),
                    FechaVisitaUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRestauranteVisitados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioRestauranteVisitados_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsuarioRestauranteVisitados_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRestricciones",
                columns: table => new
                {
                    RestriccionesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuariosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRestricciones", x => new { x.RestriccionesId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_UsuarioRestricciones_Restricciones_RestriccionesId",
                        column: x => x.RestriccionesId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRestricciones_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioNombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrupoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoGustos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrupoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GustoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoGustos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrupoGustos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrupoGustos_Gustos_GustoId",
                        column: x => x.GustoId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvitacionesGrupos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GrupoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioInvitadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioInvitadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInvitacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MensajePersonalizado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitacionesGrupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitacionesGrupos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvitacionesGrupos_Usuarios_UsuarioInvitadoId",
                        column: x => x.UsuarioInvitadoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitacionesGrupos_Usuarios_UsuarioInvitadorId",
                        column: x => x.UsuarioInvitadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MiembrosGrupos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrupoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaUnion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    EsAdministrador = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiembrosGrupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiembrosGrupos_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MiembrosGrupos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioDestinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_InvitacionesGrupos_InvitacionId",
                        column: x => x.InvitacionId,
                        principalTable: "InvitacionesGrupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                        column: x => x.UsuarioDestinoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CondicionesMedicas",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("44444444-0001-0001-0001-000000000001"), "Diabetes" },
                    { new Guid("44444444-0001-0001-0001-000000000002"), "Hipertensión" },
                    { new Guid("44444444-0001-0001-0001-000000000003"), "Colesterol alto" },
                    { new Guid("44444444-0001-0001-0001-000000000004"), "Gastritis" },
                    { new Guid("44444444-0001-0001-0001-000000000005"), "Enfermedad celíaca" },
                    { new Guid("44444444-0001-0001-0001-000000000006"), "Intolerancia a la lactosa" },
                    { new Guid("44444444-0001-0001-0001-000000000007"), "Alergia a mariscos" },
                    { new Guid("44444444-0001-0001-0001-000000000008"), "Alergia a frutos secos" },
                    { new Guid("44444444-0001-0001-0001-000000000009"), "Alergia al huevo" },
                    { new Guid("44444444-0001-0001-0001-000000000010"), "Síndrome del intestino irritable" },
                    { new Guid("44444444-0001-0001-0001-000000000011"), "Gota" },
                    { new Guid("44444444-0001-0001-0001-000000000012"), "Ansiedad (sensibilidad a cafeína)" },
                    { new Guid("44444444-0001-0001-0001-000000000013"), "Vegetariano" },
                    { new Guid("44444444-0001-0001-0001-000000000014"), "Vegano" }
                });

            migrationBuilder.InsertData(
                table: "Gustos",
                columns: new[] { "Id", "ImagenUrl", "Nombre" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0001-0001-000000000001"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pizza.jpg?alt=media&token=1e4e7fea-31d3-4e04-ae50-1ebe29fd16f2", "Pizza" },
                    { new Guid("22222222-0001-0001-0001-000000000002"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sushi.jpg?alt=media&token=9dfd9b64-8455-4206-a5ec-090c935e86e7", "Sushi" },
                    { new Guid("22222222-0001-0001-0001-000000000003"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/paella.jpg?alt=media&token=5cfd79d4-7e92-452e-a7c4-899b374d3ea8", "Paella" },
                    { new Guid("22222222-0001-0001-0001-000000000004"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/milanesa-con-papas-fritas.jpg?alt=media&token=d2ca59bc-6360-4378-919a-886b0c0e93e0", "Milanesa con papas" },
                    { new Guid("22222222-0001-0001-0001-000000000005"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tacos.jpg?alt=media&token=431ae163-15e9-41d0-8fa6-6f79e9862150", "Tacos" },
                    { new Guid("22222222-0001-0001-0001-000000000006"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada-cesar.jpg?alt=media&token=a6b5eaf0-be77-4716-8b11-18f3774f004f", "Ensalada César" },
                    { new Guid("22222222-0001-0001-0001-000000000007"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ramen.jpg?alt=media&token=886fdc48-3d43-46fd-9911-48b1966da347", "Ramen japonés" },
                    { new Guid("22222222-0001-0001-0001-000000000008"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/empanadas.png?alt=media&token=7438d05a-c0be-4da0-aea6-b6ab26f7f621", "Empanadas" },
                    { new Guid("22222222-0001-0001-0001-000000000009"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ceviche.jpg?alt=media&token=ad28a0df-4bc0-4aa8-ae02-610526ac1152", "Ceviche" },
                    { new Guid("22222222-0001-0001-0001-000000000010"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/helado.jpg?alt=media&token=01be542d-9cc4-47f3-a27f-ae3a1b80d306", "Helado" },
                    { new Guid("22222222-0001-0001-0001-000000000011"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Hamburguesa.jpg?alt=media&token=a0fd669b-ade3-427c-b428-c743338885c8", "Hamburguesa" },
                    { new Guid("22222222-0001-0001-0001-000000000012"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/papas-fritas.jpg?alt=media&token=5b18bf54-256e-4b36-adc3-e438fa3d374c", "Papas fritas" },
                    { new Guid("22222222-0001-0001-0001-000000000013"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pollo-grill.jpg?alt=media&token=d60229b3-5e8a-4de6-9ed7-4a9622a2f3e1", "Pollo grillado" },
                    { new Guid("22222222-0001-0001-0001-000000000014"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/kebab.jpg?alt=media&token=0acd13ee-654c-4748-bbe4-695a06053f75", "Kebab" },
                    { new Guid("22222222-0001-0001-0001-000000000015"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada-verde.jpg?alt=media&token=0bb027c8-de8d-4ac4-99db-ee80fc7d0f1c", "Ensalada verde" },
                    { new Guid("22222222-0001-0001-0001-000000000016"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/asado.jpg?alt=media&token=254fbe63-39ba-4529-87bb-556381370c9a", "Asado" },
                    { new Guid("22222222-0001-0001-0001-000000000017"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sopa-verduras.jpg?alt=media&token=9858f540-0cb8-4759-a7d5-a6743144863e", "Sopa de verduras" },
                    { new Guid("22222222-0001-0001-0001-000000000018"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/panquques.jpg?alt=media&token=2b203013-cbcf-40f3-b266-d629466cd0b2", "Panqueques" },
                    { new Guid("22222222-0001-0001-0001-000000000019"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/cafe-con-leche.jpg?alt=media&token=1f3cde3c-e3b9-4ed0-a690-d7e4d6875447", "Café con leche" },
                    { new Guid("22222222-0001-0001-0001-000000000020"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/jugos-naturales.jpg?alt=media&token=b8afeb02-882e-4a3f-8445-14949e5871dd", "Jugo natural" },
                    { new Guid("22222222-0001-0001-0001-000000000021"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Smoothie-frutas.jpg?alt=media&token=17ddc1f6-d60c-40d4-8368-f1f003ddd62b", "Smoothie de frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000022"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/chocolates.jpg?alt=media&token=5f33a2af-cee7-4768-8b47-dda973dd9c4e", "Chocolate" },
                    { new Guid("22222222-0001-0001-0001-000000000023"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tarta-manzana.jpg?alt=media&token=c34fa589-0c37-44cb-891e-2e8aaebdb215", "Tarta de manzana" },
                    { new Guid("22222222-0001-0001-0001-000000000024"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Pescado-al-horn.png?alt=media&token=953296de-c3be-47a4-826e-31ad29cbae22", "Pescado al horno" },
                    { new Guid("22222222-0001-0001-0001-000000000025"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pasta-bolognesa.jpg?alt=media&token=7893a48f-f693-4a2f-a574-81336f91e62c", "Pasta boloñesa" },
                    { new Guid("22222222-0001-0001-0001-000000000026"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/lomo_a_la_pimienta.png?alt=media&token=4d8495a4-181b-4d50-a9c8-e95010bfb100", "Lomo a la pimienta" },
                    { new Guid("22222222-0001-0001-0001-000000000027"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada_frutas.jpg?alt=media&token=a13662fa-2ac4-40cd-b1f0-347634a991e9", "Ensalada de frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000028"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sandwich-de-huevo-con-jamon-y-queso.jpg?alt=media&token=8ac9b05d-c316-4a84-a099-9bca4f2d6a9a", "Sándwich de jamón y queso" },
                    { new Guid("22222222-0001-0001-0001-000000000029"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/huevos-revueltos-desayuno.jpeg?alt=media&token=0f21b637-b499-427c-bdb0-f0a841a76a9b", "Huevos revueltos" },
                    { new Guid("22222222-0001-0001-0001-000000000030"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tipos-de-cerveza.jpg?alt=media&token=1cfa9e77-b663-421a-b649-d52a1ba751d2", "Cerveza artesanal" },
                    { new Guid("22222222-0001-0001-0001-000000000031"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/vino_artesanal.jpg?alt=media&token=fd22ec00-7739-4776-b488-63e46c2937c5", "Vino tinto" },
                    { new Guid("22222222-0001-0001-0001-000000000032"), null, "Ensalada Caprese" },
                    { new Guid("22222222-0001-0001-0001-000000000033"), null, "Tarta de Verduras" },
                    { new Guid("22222222-0001-0001-0001-000000000034"), null, "Omelette de vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000035"), null, "Pizza Margarita" },
                    { new Guid("22222222-0001-0001-0001-000000000036"), null, "Milanesa de berenjena" },
                    { new Guid("22222222-0001-0001-0001-000000000037"), null, "Ñoquis con salsa de tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000038"), null, "Ravioles de ricota y espinaca" },
                    { new Guid("22222222-0001-0001-0001-000000000039"), null, "Fideos con pesto" },
                    { new Guid("22222222-0001-0001-0001-000000000040"), null, "Panqueques de avena con frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000041"), null, "Empanadas de humita" },
                    { new Guid("22222222-0001-0001-0001-000000000042"), null, "Lasaña vegetariana" },
                    { new Guid("22222222-0001-0001-0001-000000000043"), null, "Arroz primavera" },
                    { new Guid("22222222-0001-0001-0001-000000000044"), null, "Polenta con salsa de tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000045"), null, "Sopa de calabaza" },
                    { new Guid("22222222-0001-0001-0001-000000000046"), null, "Tortilla de papas" },
                    { new Guid("22222222-0001-0001-0001-000000000047"), null, "Quesadillas de vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000048"), null, "Bruschettas con tomate y albahaca" },
                    { new Guid("22222222-0001-0001-0001-000000000049"), null, "Pastel de papas vegetariano" },
                    { new Guid("22222222-0001-0001-0001-000000000050"), null, "Pizza cuatro quesos" },
                    { new Guid("22222222-0001-0001-0001-000000000051"), null, "Ensalada de quinoa con vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000052"), null, "Curry de vegetales con arroz" },
                    { new Guid("22222222-0001-0001-0001-000000000053"), null, "Hamburguesa de lentejas" },
                    { new Guid("22222222-0001-0001-0001-000000000054"), null, "Sopa crema de zapallo con leche vegetal" },
                    { new Guid("22222222-0001-0001-0001-000000000055"), null, "Arroz frito con tofu" },
                    { new Guid("22222222-0001-0001-0001-000000000056"), null, "Guiso de lentejas vegano" },
                    { new Guid("22222222-0001-0001-0001-000000000057"), null, "Pan integral con palta y tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000058"), null, "Panqueques de banana sin huevo" },
                    { new Guid("22222222-0001-0001-0001-000000000059"), null, "Wrap de falafel con hummus" },
                    { new Guid("22222222-0001-0001-0001-000000000060"), null, "Brownie vegano(con harina integral y aceite de coco" },
                    { new Guid("22222222-0001-0001-0001-000000000061"), null, "Tarta vegana de calabaza" },
                    { new Guid("22222222-0001-0001-0001-000000000062"), null, "Empanadas veganas" },
                    { new Guid("22222222-0001-0001-0001-000000000063"), null, "Tacos veganos" },
                    { new Guid("22222222-0001-0001-0001-000000000064"), null, "Fideos de arroz con verduras salteadas" }
                });

            migrationBuilder.InsertData(
                table: "Restricciones",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("33333333-0001-0001-0001-000000000001"), "Sin gluten" },
                    { new Guid("33333333-0001-0001-0001-000000000002"), "Sin lactosa" },
                    { new Guid("33333333-0001-0001-0001-000000000003"), "Sin azúcar" },
                    { new Guid("33333333-0001-0001-0001-000000000004"), "Sin sal" },
                    { new Guid("33333333-0001-0001-0001-000000000005"), "Sin mariscos" },
                    { new Guid("33333333-0001-0001-0001-000000000006"), "Sin carne roja" },
                    { new Guid("33333333-0001-0001-0001-000000000007"), "Sin frito" },
                    { new Guid("33333333-0001-0001-0001-000000000008"), "Sin picante" },
                    { new Guid("33333333-0001-0001-0001-000000000009"), "Sin cafeína" },
                    { new Guid("33333333-0001-0001-0001-000000000010"), "Sin alcohol" },
                    { new Guid("33333333-0001-0001-0001-000000000011"), "Sin soja" },
                    { new Guid("33333333-0001-0001-0001-000000000012"), "Sin frutos secos" }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Nombre", "Tipo" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Gluten", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), "Lácteo", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), "Azúcar", "Nutriente" },
                    { new Guid("11111111-1111-1111-1111-111111111114"), "Sal", "Nutriente" },
                    { new Guid("11111111-1111-1111-1111-111111111115"), "Mariscos", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111116"), "Carne roja", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111117"), "Carne blanca", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111118"), "Vegetal", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111119"), "Fruta", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111120"), "Frito", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111121"), "Picante", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111122"), "Procesado", "Categoria" },
                    { new Guid("11111111-1111-1111-1111-111111111123"), "Pescado", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111124"), "Grasa", "Nutriente" },
                    { new Guid("11111111-1111-1111-1111-111111111125"), "Cafeína", "Nutriente" },
                    { new Guid("11111111-1111-1111-1111-111111111126"), "Harina", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111127"), "Huevos", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111128"), "Frutos secos", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111129"), "Soja", "Ingrediente" },
                    { new Guid("11111111-1111-1111-1111-111111111130"), "Alcohol", "Nutriente" }
                });

            migrationBuilder.InsertData(
                table: "CondicionMedicaTags",
                columns: new[] { "CondicionesMedicasId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("44444444-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("44444444-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("44444444-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("44444444-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("44444444-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("44444444-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("44444444-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111115") },
                    { new Guid("44444444-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111128") },
                    { new Guid("44444444-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("44444444-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("44444444-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("44444444-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111125") },
                    { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111117") },
                    { new Guid("44444444-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111117") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("44444444-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111127") }
                });

            migrationBuilder.InsertData(
                table: "GustoTags",
                columns: new[] { "GustosId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("22222222-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111115") },
                    { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111115") },
                    { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("22222222-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111117") },
                    { new Guid("22222222-0001-0001-0001-000000000013"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("22222222-0001-0001-0001-000000000014"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000015"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000016"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000017"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000017"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000018"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000019"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000019"), new Guid("11111111-1111-1111-1111-111111111125") },
                    { new Guid("22222222-0001-0001-0001-000000000020"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000021"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000021"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000022"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000022"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000023"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000024"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000024"), new Guid("11111111-1111-1111-1111-111111111123") },
                    { new Guid("22222222-0001-0001-0001-000000000025"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000025"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000026"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000026"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("22222222-0001-0001-0001-000000000027"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000028"), new Guid("11111111-1111-1111-1111-111111111122") },
                    { new Guid("22222222-0001-0001-0001-000000000029"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000030"), new Guid("11111111-1111-1111-1111-111111111130") },
                    { new Guid("22222222-0001-0001-0001-000000000031"), new Guid("11111111-1111-1111-1111-111111111130") },
                    { new Guid("22222222-0001-0001-0001-000000000032"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000032"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000033"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000033"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000034"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000034"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000035"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000036"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000037"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000037"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000038"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000039"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000040"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000041"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000042"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000043"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000044"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-0001-0001-0001-000000000044"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000045"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000046"), new Guid("11111111-1111-1111-1111-111111111127") },
                    { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000047"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000048"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000048"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000049"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000049"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000050"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000051"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000052"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000052"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("22222222-0001-0001-0001-000000000053"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000053"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000054"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("22222222-0001-0001-0001-000000000055"), new Guid("11111111-1111-1111-1111-111111111129") },
                    { new Guid("22222222-0001-0001-0001-000000000056"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000057"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000057"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000058"), new Guid("11111111-1111-1111-1111-111111111119") },
                    { new Guid("22222222-0001-0001-0001-000000000058"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000059"), new Guid("11111111-1111-1111-1111-111111111129") },
                    { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111124") },
                    { new Guid("22222222-0001-0001-0001-000000000060"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000061"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000061"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000062"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000062"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000063"), new Guid("11111111-1111-1111-1111-111111111118") },
                    { new Guid("22222222-0001-0001-0001-000000000063"), new Guid("11111111-1111-1111-1111-111111111126") },
                    { new Guid("22222222-0001-0001-0001-000000000064"), new Guid("11111111-1111-1111-1111-111111111118") }
                });

            migrationBuilder.InsertData(
                table: "RestriccionTags",
                columns: new[] { "RestriccionesId", "TagsId" },
                values: new object[,]
                {
                    { new Guid("33333333-0001-0001-0001-000000000001"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("33333333-0001-0001-0001-000000000002"), new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("33333333-0001-0001-0001-000000000003"), new Guid("11111111-1111-1111-1111-111111111113") },
                    { new Guid("33333333-0001-0001-0001-000000000004"), new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("33333333-0001-0001-0001-000000000005"), new Guid("11111111-1111-1111-1111-111111111115") },
                    { new Guid("33333333-0001-0001-0001-000000000006"), new Guid("11111111-1111-1111-1111-111111111116") },
                    { new Guid("33333333-0001-0001-0001-000000000007"), new Guid("11111111-1111-1111-1111-111111111120") },
                    { new Guid("33333333-0001-0001-0001-000000000008"), new Guid("11111111-1111-1111-1111-111111111121") },
                    { new Guid("33333333-0001-0001-0001-000000000009"), new Guid("11111111-1111-1111-1111-111111111125") },
                    { new Guid("33333333-0001-0001-0001-000000000010"), new Guid("11111111-1111-1111-1111-111111111130") },
                    { new Guid("33333333-0001-0001-0001-000000000011"), new Guid("11111111-1111-1111-1111-111111111129") },
                    { new Guid("33333333-0001-0001-0001-000000000012"), new Guid("11111111-1111-1111-1111-111111111128") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_GrupoId",
                table: "ChatMessages",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UsuarioId",
                table: "ChatMessages",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CondicionMedicaTags_TagsId",
                table: "CondicionMedicaTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoGustos_GrupoId",
                table: "GrupoGustos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoGustos_GustoId",
                table: "GrupoGustos",
                column: "GustoId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_AdministradorId",
                table: "Grupos",
                column: "AdministradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_CodigoInvitacion",
                table: "Grupos",
                column: "CodigoInvitacion",
                unique: true,
                filter: "[CodigoInvitacion] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GustoTags_TagsId",
                table: "GustoTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitacionesGrupos_GrupoId",
                table: "InvitacionesGrupos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitacionesGrupos_UsuarioInvitadoId",
                table: "InvitacionesGrupos",
                column: "UsuarioInvitadoId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitacionesGrupos_UsuarioInvitadorId",
                table: "InvitacionesGrupos",
                column: "UsuarioInvitadorId");

            migrationBuilder.CreateIndex(
                name: "IX_MiembrosGrupos_GrupoId_UsuarioId",
                table: "MiembrosGrupos",
                columns: new[] { "GrupoId", "UsuarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiembrosGrupos_UsuarioId",
                table: "MiembrosGrupos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_InvitacionId",
                table: "Notificaciones",
                column: "InvitacionId",
                unique: true,
                filter: "[InvitacionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioDestinoId",
                table: "Notificaciones",
                column: "UsuarioDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReseñasRestaurantes_RestauranteId",
                table: "ReseñasRestaurantes",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_RestauranteEspecialidades_RestauranteId",
                table: "RestauranteEspecialidades",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_RestauranteGustos_restaurantesId",
                table: "RestauranteGustos",
                column: "restaurantesId");

            migrationBuilder.CreateIndex(
                name: "IX_RestImagen_Rest_Tipo_Orden",
                table: "RestauranteImagenes",
                columns: new[] { "RestauranteId", "Tipo", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_RestMenu_RestauranteId",
                table: "RestauranteMenus",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_RestauranteRestricciones_RestriccionesQueRespetaId",
                table: "RestauranteRestricciones",
                column: "RestriccionesQueRespetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_Latitud_Longitud",
                table: "Restaurantes",
                columns: new[] { "Latitud", "Longitud" });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_NombreNormalizado",
                table: "Restaurantes",
                column: "NombreNormalizado");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_PrimaryType",
                table: "Restaurantes",
                column: "PrimaryType");

            migrationBuilder.CreateIndex(
                name: "UX_Restaurantes_PlaceId",
                table: "Restaurantes",
                column: "PlaceId",
                unique: true,
                filter: "[PlaceId] IS NOT NULL AND [PlaceId] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionTags_TagsId",
                table: "RestriccionTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAmistad_DestinatarioId",
                table: "SolicitudesAmistad",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAmistad_RemitenteId",
                table: "SolicitudesAmistad",
                column: "RemitenteId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCondicionesMedicas_UsuariosId",
                table: "UsuarioCondicionesMedicas",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioGustos_UsuariosId",
                table: "UsuarioGustos",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRestauranteVisitados_PlaceId",
                table: "UsuarioRestauranteVisitados",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRestauranteVisitados_RestauranteId",
                table: "UsuarioRestauranteVisitados",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRestauranteVisitados_UsuarioId",
                table: "UsuarioRestauranteVisitados",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRestricciones_UsuariosId",
                table: "UsuarioRestricciones",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_FirebaseUid",
                table: "Usuarios",
                column: "FirebaseUid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdUsuario",
                table: "Usuarios",
                column: "IdUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "CondicionMedicaTags");

            migrationBuilder.DropTable(
                name: "GrupoGustos");

            migrationBuilder.DropTable(
                name: "GustoTags");

            migrationBuilder.DropTable(
                name: "MiembrosGrupos");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "ReseñasRestaurantes");

            migrationBuilder.DropTable(
                name: "RestauranteEspecialidades");

            migrationBuilder.DropTable(
                name: "RestauranteGustos");

            migrationBuilder.DropTable(
                name: "RestauranteImagenes");

            migrationBuilder.DropTable(
                name: "RestauranteMenus");

            migrationBuilder.DropTable(
                name: "RestaurantePlatos");

            migrationBuilder.DropTable(
                name: "RestauranteRestricciones");

            migrationBuilder.DropTable(
                name: "RestriccionTags");

            migrationBuilder.DropTable(
                name: "SolicitudesAmistad");

            migrationBuilder.DropTable(
                name: "UsuarioCondicionesMedicas");

            migrationBuilder.DropTable(
                name: "UsuarioGustos");

            migrationBuilder.DropTable(
                name: "UsuarioRestauranteVisitados");

            migrationBuilder.DropTable(
                name: "UsuarioRestricciones");

            migrationBuilder.DropTable(
                name: "InvitacionesGrupos");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "CondicionesMedicas");

            migrationBuilder.DropTable(
                name: "Gustos");

            migrationBuilder.DropTable(
                name: "Restaurantes");

            migrationBuilder.DropTable(
                name: "Restricciones");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
