using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
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
                    RegistroInicialCompleto = table.Column<bool>(type: "bit", nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    Plan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
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
                name: "Restaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropietarioUid = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DuenoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    MenuProcesado = table.Column<bool>(type: "bit", nullable: true),
                    MenuError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false, defaultValue: "restaurant"),
                    TypesJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    ImagenUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Valoracion = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restaurantes_Usuarios_DuenoId",
                        column: x => x.DuenoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                name: "SolicitudesRestaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitud = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitud = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HorariosJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GustosIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RestriccionesIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesRestaurantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesRestaurantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
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
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                        principalColumn: "Id");
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
                    EsAdministrador = table.Column<bool>(type: "bit", nullable: false),
                    afectarRecomendacion = table.Column<bool>(type: "bit", nullable: false)
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
                name: "OpinionRestaurante",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AutorExterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FuenteExterna = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagenAutorExterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valoracion = table.Column<double>(type: "float", nullable: false),
                    FechaVisita = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Opinion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotivoVisita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MesAnioVisita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsImportada = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpinionRestaurante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpinionRestaurante_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpinionRestaurante_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
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
                name: "RestauranteEstadisticas",
                columns: table => new
                {
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalTop3Individual = table.Column<int>(type: "int", nullable: false),
                    TotalTop3Grupo = table.Column<int>(type: "int", nullable: false),
                    TotalVisitasPerfil = table.Column<int>(type: "int", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimaActualizacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestauranteEstadisticas", x => x.RestauranteId);
                    table.ForeignKey(
                        name: "FK_RestauranteEstadisticas_Restaurantes_RestauranteId",
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
                name: "UsuarioRestauranteFavoritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAgregado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRestauranteFavoritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioRestauranteFavoritos_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRestauranteFavoritos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
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
                name: "Votaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrupoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RestauranteGanadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votaciones_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votaciones_Restaurantes_RestauranteGanadorId",
                        column: x => x.RestauranteGanadorId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudRestaurante_Gustos",
                columns: table => new
                {
                    GustosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudRestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudRestaurante_Gustos", x => new { x.GustosId, x.SolicitudRestauranteId });
                    table.ForeignKey(
                        name: "FK_SolicitudRestaurante_Gustos_Gustos_GustosId",
                        column: x => x.GustosId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudRestaurante_Gustos_SolicitudesRestaurantes_SolicitudRestauranteId",
                        column: x => x.SolicitudRestauranteId,
                        principalTable: "SolicitudesRestaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudRestaurante_Restricciones",
                columns: table => new
                {
                    RestriccionesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudRestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudRestaurante_Restricciones", x => new { x.RestriccionesId, x.SolicitudRestauranteId });
                    table.ForeignKey(
                        name: "FK_SolicitudRestaurante_Restricciones_Restricciones_RestriccionesId",
                        column: x => x.RestriccionesId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudRestaurante_Restricciones_SolicitudesRestaurantes_SolicitudRestauranteId",
                        column: x => x.SolicitudRestauranteId,
                        principalTable: "SolicitudesRestaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudRestauranteImagenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudRestauranteImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudRestauranteImagenes_SolicitudesRestaurantes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesRestaurantes",
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

            migrationBuilder.CreateTable(
                name: "GrupoGustos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrupoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GustoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MiembroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_GrupoGustos_MiembrosGrupos_MiembroId",
                        column: x => x.MiembroId,
                        principalTable: "MiembrosGrupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpinionesFotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpinionRestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpinionesFotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpinionesFotos_OpinionRestaurante_OpinionRestauranteId",
                        column: x => x.OpinionRestauranteId,
                        principalTable: "OpinionRestaurante",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VotacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaVoto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votos_Votaciones_VotacionId",
                        column: x => x.VotacionId,
                        principalTable: "Votaciones",
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
                    { new Guid("22222222-0001-0001-0001-000000000001"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpizza%20(1)%20(1).jpg?alt=media&token=acae5ec9-56ff-4b1e-9f52-c6e3aa1c0465", "Pizza" },
                    { new Guid("22222222-0001-0001-0001-000000000002"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fsushi%20(1).jpg?alt=media&token=699bb1f7-9ca8-467c-a07c-c25b85e155dd", "Sushi" },
                    { new Guid("22222222-0001-0001-0001-000000000003"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpaella%20(1)%20(1).jpg?alt=media&token=8d28a0aa-4e53-4552-ad79-4007f27ea6ef", "Paella" },
                    { new Guid("22222222-0001-0001-0001-000000000004"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/milanesa-con-papas-fritas.jpg?alt=media&token=d2ca59bc-6360-4378-919a-886b0c0e93e0", "Milanesa con papas" },
                    { new Guid("22222222-0001-0001-0001-000000000005"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tacos.jpg?alt=media&token=431ae163-15e9-41d0-8fa6-6f79e9862150", "Tacos" },
                    { new Guid("22222222-0001-0001-0001-000000000006"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fensalada-cesar%20(1).jpg?alt=media&token=c478c0b8-7b06-40d3-b3b6-b4d8a1c7af45", "Ensalada César" },
                    { new Guid("22222222-0001-0001-0001-000000000007"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Framen%20(1).jpg?alt=media&token=f50e95b0-8f37-499a-803e-2f9ce530d68a", "Ramen japonés" },
                    { new Guid("22222222-0001-0001-0001-000000000008"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fempanadas.jpg?alt=media&token=07a8e917-a280-4345-9717-fe0b707dc8e7", "Empanadas" },
                    { new Guid("22222222-0001-0001-0001-000000000009"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ceviche.jpg?alt=media&token=ad28a0df-4bc0-4aa8-ae02-610526ac1152", "Ceviche" },
                    { new Guid("22222222-0001-0001-0001-000000000010"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fhelado%20(1).jpg?alt=media&token=eef033a6-344a-41af-904a-7ff1116671c3", "Helado" },
                    { new Guid("22222222-0001-0001-0001-000000000011"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Hamburguesa.jpg?alt=media&token=a0fd669b-ade3-427c-b428-c743338885c8", "Hamburguesa" },
                    { new Guid("22222222-0001-0001-0001-000000000012"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/papas-fritas.jpg?alt=media&token=5b18bf54-256e-4b36-adc3-e438fa3d374c", "Papas fritas" },
                    { new Guid("22222222-0001-0001-0001-000000000013"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fpollo-grill%20(1)%20(1).jpg?alt=media&token=22febf47-6138-4f88-a0a5-d131ef67f44c", "Pollo grillado" },
                    { new Guid("22222222-0001-0001-0001-000000000014"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/kebab.jpg?alt=media&token=0acd13ee-654c-4748-bbe4-695a06053f75", "Kebab" },
                    { new Guid("22222222-0001-0001-0001-000000000015"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada-verde.jpg?alt=media&token=0bb027c8-de8d-4ac4-99db-ee80fc7d0f1c", "Ensalada verde" },
                    { new Guid("22222222-0001-0001-0001-000000000016"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fasado%20(1).jpg?alt=media&token=86e67902-4e82-4d36-87df-b6f4973e9b61", "Asado" },
                    { new Guid("22222222-0001-0001-0001-000000000017"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fsopa-verduras%20(1).jpg?alt=media&token=a9f74b05-2726-4a12-89fc-51e9ab99f895", "Sopa de verduras" },
                    { new Guid("22222222-0001-0001-0001-000000000018"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/panquques.jpg?alt=media&token=2b203013-cbcf-40f3-b266-d629466cd0b2", "Panqueques" },
                    { new Guid("22222222-0001-0001-0001-000000000019"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/cafe-con-leche.jpg?alt=media&token=1f3cde3c-e3b9-4ed0-a690-d7e4d6875447", "Café con leche" },
                    { new Guid("22222222-0001-0001-0001-000000000020"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/jugos-naturales.jpg?alt=media&token=b8afeb02-882e-4a3f-8445-14949e5871dd", "Jugo natural" },
                    { new Guid("22222222-0001-0001-0001-000000000021"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/Smoothie-frutas.jpg?alt=media&token=17ddc1f6-d60c-40d4-8368-f1f003ddd62b", "Smoothie de frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000022"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fchocolates%20(1).jpg?alt=media&token=52113b04-41a3-4e40-85bf-c6887227528c", "Chocolate" },
                    { new Guid("22222222-0001-0001-0001-000000000023"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Ftarta-manzana%20(1).jpg?alt=media&token=a16c689c-6cbb-4e77-a9f3-5bf448dc6450", "Tarta de manzana" },
                    { new Guid("22222222-0001-0001-0001-000000000024"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2FPescado-al-horn.jpg?alt=media&token=dd7a4dee-6229-4766-83e1-2647ab36b64c", "Pescado al horno" },
                    { new Guid("22222222-0001-0001-0001-000000000025"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/pasta-bolognesa.jpg?alt=media&token=7893a48f-f693-4a2f-a574-81336f91e62c", "Pasta boloñesa" },
                    { new Guid("22222222-0001-0001-0001-000000000026"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Flomo_a_la_pimienta.jpg?alt=media&token=1b1b8e30-083a-4ad2-854b-7903a4f750c9", "Lomo a la pimienta" },
                    { new Guid("22222222-0001-0001-0001-000000000027"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/ensalada_frutas.jpg?alt=media&token=a13662fa-2ac4-40cd-b1f0-347634a991e9", "Ensalada de frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000028"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/sandwich-de-huevo-con-jamon-y-queso.jpg?alt=media&token=8ac9b05d-c316-4a84-a099-9bca4f2d6a9a", "Sándwich de jamón y queso" },
                    { new Guid("22222222-0001-0001-0001-000000000029"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%20optimizado%2Fhuevos-revueltos-desayuno.jpg?alt=media&token=c08b7124-4f73-4c68-b85e-7f8949b65688", "Huevos revueltos" },
                    { new Guid("22222222-0001-0001-0001-000000000030"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/tipos-de-cerveza.jpg?alt=media&token=1cfa9e77-b663-421a-b649-d52a1ba751d2", "Cerveza artesanal" },
                    { new Guid("22222222-0001-0001-0001-000000000031"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/vino_artesanal.jpg?alt=media&token=fd22ec00-7739-4776-b488-63e46c2937c5", "Vino tinto" },
                    { new Guid("22222222-0001-0001-0001-000000000032"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fensalada-caprese.jpg?alt=media&token=6bc93fe3-acc5-46b4-be63-c1de7b621854", "Ensalada Caprese" },
                    { new Guid("22222222-0001-0001-0001-000000000033"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftarta-verduras.jpg?alt=media&token=65092fe1-acfa-4c5a-84c4-ebcf00ad6068", "Tarta de Verduras" },
                    { new Guid("22222222-0001-0001-0001-000000000034"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fomelette-de-verduras.jpg?alt=media&token=25b514e3-5f87-43a0-99b0-83b0b9af243f", "Omelette de vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000035"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fpizza-margarita.jpg?alt=media&token=0ae93afc-274e-4a4b-879a-8d288cac8d3a", "Pizza Margarita" },
                    { new Guid("22222222-0001-0001-0001-000000000036"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FMILANESAS-DE-BERENJENA-CON-PURE-DE-ARVEJAS-05%20(1).jpg?alt=media&token=f21fab62-9138-450d-be46-faf0acf43ef1", "Milanesa de berenjena" },
                    { new Guid("22222222-0001-0001-0001-000000000037"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fnoquis-caseros-con-salsa-de-tomate-1200x900.jpg?alt=media&token=35667f32-5775-41b6-9d29-aed9acf2a64b", "Ñoquis con salsa de tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000038"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FRavioles-ricota-espinaca%20(1).jpg?alt=media&token=89d4e8ba-7db8-40d5-8d1a-09cf16216af8", "Ravioles de ricota y espinaca" },
                    { new Guid("22222222-0001-0001-0001-000000000039"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ffideos_con_pesto.jpg?alt=media&token=47cbf2d7-53f2-4710-88a8-ef8b43fb3dfc", "Fideos con pesto" },
                    { new Guid("22222222-0001-0001-0001-000000000040"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPanquequavenarutas.jpg?alt=media&token=0d4dcf14-a9e3-48c1-a6ff-06118c259555", "Panqueques de avena con frutas" },
                    { new Guid("22222222-0001-0001-0001-000000000041"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fempanda_humita.jpg?alt=media&token=862cd874-b8b2-4793-93f5-284268db5f6d", "Empanadas de humita" },
                    { new Guid("22222222-0001-0001-0001-000000000042"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FLasa%C3%B1a-vegetariana.jpg?alt=media&token=2ad7e9ef-a341-43d3-b135-9b22b2f9d0ba", "Lasaña vegetariana" },
                    { new Guid("22222222-0001-0001-0001-000000000043"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FArrozprimavera.jpg?alt=media&token=3872ad34-aec3-4610-af74-88a5ed01894f", "Arroz primavera" },
                    { new Guid("22222222-0001-0001-0001-000000000044"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPolenta-salsa-tomate.jpg?alt=media&token=9869796e-f39d-4536-afe4-b79131bdcaa9", "Polenta con salsa de tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000045"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fsopa-de-calabaza_web.jpg?alt=media&token=06b7d888-fea8-4f88-8447-c8c4494018e9", "Sopa de calabaza" },
                    { new Guid("22222222-0001-0001-0001-000000000046"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftortilla-de-papas-con-EPMZH233TBBEBETIIAYLJQ57JU.jpg?alt=media&token=42079a0c-02c9-4de4-8e9e-eff9d9c59d9f", "Tortilla de papas" },
                    { new Guid("22222222-0001-0001-0001-000000000047"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FQuesadillas-vegetales.jpg?alt=media&token=2f5fcc35-826f-4134-a50f-a7a81f2e20d2", "Quesadillas de vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000048"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FBruschetta-tomate-albahaca.jpg?alt=media&token=3dc0476c-f168-40f1-afa1-d557e2ab4f90", "Bruschettas con tomate y albahaca" },
                    { new Guid("22222222-0001-0001-0001-000000000049"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPastel-papas-vegetariano.jpg?alt=media&token=81f3f01a-f6cc-4489-91ca-d191d30a5567", "Pastel de papas vegetariano" },
                    { new Guid("22222222-0001-0001-0001-000000000050"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPizza-cuatro-quesos%20(1).jpg?alt=media&token=d92594ca-b9f7-42ac-b2b1-ceb16f269740", "Pizza cuatro quesos" },
                    { new Guid("22222222-0001-0001-0001-000000000051"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FEnsalada-de-quinoa-con-verduras.jpg?alt=media&token=aec93b8a-c0ae-431e-b006-8db27840fdc5", "Ensalada de quinoa con vegetales" },
                    { new Guid("22222222-0001-0001-0001-000000000052"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Farroz-al-curry-con-2HA57GEWZJFM5CD27MHLFJ6ZMU.jpg?alt=media&token=65998321-d8c5-4dea-ba17-dcb311f2cf77", "Curry de vegetales con arroz" },
                    { new Guid("22222222-0001-0001-0001-000000000053"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fhamburguesa-lentejas-7.jpg?alt=media&token=a418381f-009f-47dc-ac6d-1713c94df7bf", "Hamburguesa de lentejas" },
                    { new Guid("22222222-0001-0001-0001-000000000054"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FSopa-crema-zapallo-leche-vegetal.jpg?alt=media&token=fa58ead1-8dbd-4121-a5e0-5f9428713a57", "Sopa crema de zapallo con leche vegetal" },
                    { new Guid("22222222-0001-0001-0001-000000000055"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Farroz_frito_con_tofu_62074_orig.jpg?alt=media&token=1e451e0b-8eef-4e0d-a761-c9364e6ec4c9", "Arroz frito con tofu" },
                    { new Guid("22222222-0001-0001-0001-000000000056"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FGuiso-lentejas-vegano.jpg?alt=media&token=3ef78d85-c7e4-4fb6-b1cc-8c00b183dea0", "Guiso de lentejas vegano" },
                    { new Guid("22222222-0001-0001-0001-000000000057"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fpan_con_palta_tomate.jpg?alt=media&token=2c63b3e3-4092-46f2-b852-4e382a86f452", "Pan integral con palta y tomate" },
                    { new Guid("22222222-0001-0001-0001-000000000058"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FPanqueques-banana-sin-huevo.jpg?alt=media&token=ccea698a-3c15-41d0-82dd-5080f3a91210", "Panqueques de banana sin huevo" },
                    { new Guid("22222222-0001-0001-0001-000000000059"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FWrap-falafel-hummus.jpg?alt=media&token=26a4a908-3460-4eb2-b372-077acf14fe1b", "Wrap de falafel con hummus" },
                    { new Guid("22222222-0001-0001-0001-000000000060"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FBrownievegano(con%20harina%20integra%20aceite%20de%20coco.jpg?alt=media&token=96a9ce83-43bf-4d92-9fbb-d6c9bdd0701c", "Brownie vegano(con harina integral y aceite de coco" },
                    { new Guid("22222222-0001-0001-0001-000000000061"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftarta-de-calabaza-vegana-pumpkin-pie-saludable.jpg?alt=media&token=21f2a9f5-33d6-4968-9963-21615ac588a6", "Tarta vegana de calabaza" },
                    { new Guid("22222222-0001-0001-0001-000000000062"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Fempanadillas2-1024x683.jpg?alt=media&token=20a2c707-ca29-4a1e-a194-9be0fa3424dd", "Empanadas veganas" },
                    { new Guid("22222222-0001-0001-0001-000000000063"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2Ftacos_veganos.jpg?alt=media&token=9437ffc6-8a1f-4c93-89f3-33f0e4d35ebf", "Tacos veganos" },
                    { new Guid("22222222-0001-0001-0001-000000000064"), "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/gustos1%2FFideos-arrozrerdurassalteadas.jpg?alt=media&token=a17821fc-210e-4078-856a-d892c75b5a93", "Fideos de arroz con verduras salteadas" }
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
                name: "IX_GrupoGustos_MiembroId",
                table: "GrupoGustos",
                column: "MiembroId");

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
                name: "IX_OpinionesFotos_OpinionRestauranteId",
                table: "OpinionesFotos",
                column: "OpinionRestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_OpinionRestaurante_RestauranteId",
                table: "OpinionRestaurante",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_OpinionRestaurante_UsuarioId",
                table: "OpinionRestaurante",
                column: "UsuarioId");

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
                column: "RestauranteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestauranteRestricciones_RestriccionesQueRespetaId",
                table: "RestauranteRestricciones",
                column: "RestriccionesQueRespetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_DuenoId",
                table: "Restaurantes",
                column: "DuenoId");

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
                name: "IX_SolicitudesRestaurantes_Estado",
                table: "SolicitudesRestaurantes",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRestaurantes_UsuarioId",
                table: "SolicitudesRestaurantes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudRestaurante_Gustos_SolicitudRestauranteId",
                table: "SolicitudRestaurante_Gustos",
                column: "SolicitudRestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudRestaurante_Restricciones_SolicitudRestauranteId",
                table: "SolicitudRestaurante_Restricciones",
                column: "SolicitudRestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudRestauranteImagenes_SolicitudId",
                table: "SolicitudRestauranteImagenes",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCondicionesMedicas_UsuariosId",
                table: "UsuarioCondicionesMedicas",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioGustos_UsuariosId",
                table: "UsuarioGustos",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRestauranteFavoritos_RestauranteId",
                table: "UsuarioRestauranteFavoritos",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRestauranteFavoritos_UsuarioId_RestauranteId",
                table: "UsuarioRestauranteFavoritos",
                columns: new[] { "UsuarioId", "RestauranteId" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Votaciones_GrupoId_Estado",
                table: "Votaciones",
                columns: new[] { "GrupoId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Votaciones_RestauranteGanadorId",
                table: "Votaciones",
                column: "RestauranteGanadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_RestauranteId",
                table: "Votos",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_UsuarioId",
                table: "Votos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_VotacionId_UsuarioId",
                table: "Votos",
                columns: new[] { "VotacionId", "UsuarioId" },
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
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "OpinionesFotos");

            migrationBuilder.DropTable(
                name: "RestauranteEspecialidades");

            migrationBuilder.DropTable(
                name: "RestauranteEstadisticas");

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
                name: "SolicitudRestaurante_Gustos");

            migrationBuilder.DropTable(
                name: "SolicitudRestaurante_Restricciones");

            migrationBuilder.DropTable(
                name: "SolicitudRestauranteImagenes");

            migrationBuilder.DropTable(
                name: "UsuarioCondicionesMedicas");

            migrationBuilder.DropTable(
                name: "UsuarioGustos");

            migrationBuilder.DropTable(
                name: "UsuarioRestauranteFavoritos");

            migrationBuilder.DropTable(
                name: "UsuarioRestauranteVisitados");

            migrationBuilder.DropTable(
                name: "UsuarioRestricciones");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "MiembrosGrupos");

            migrationBuilder.DropTable(
                name: "InvitacionesGrupos");

            migrationBuilder.DropTable(
                name: "OpinionRestaurante");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "SolicitudesRestaurantes");

            migrationBuilder.DropTable(
                name: "CondicionesMedicas");

            migrationBuilder.DropTable(
                name: "Gustos");

            migrationBuilder.DropTable(
                name: "Restricciones");

            migrationBuilder.DropTable(
                name: "Votaciones");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Restaurantes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
