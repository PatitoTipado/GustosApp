using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirebaseUid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FotoPerfilUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
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
                name: "InvitacionesGrupos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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

            migrationBuilder.InsertData(
                table: "CondicionesMedicas",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), "Diabetes" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), "Hipertensión" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Obesidad" },
                    { new Guid("33333333-3333-3333-3333-333333333334"), "Gastritis" },
                    { new Guid("33333333-3333-3333-3333-333333333335"), "Hígado graso" },
                    { new Guid("33333333-3333-3333-3333-333333333336"), "Anemia" },
                    { new Guid("33333333-3333-3333-3333-333333333337"), "Síndrome del intestino irritable" },
                    { new Guid("33333333-3333-3333-3333-333333333338"), "Insuficiencia renal" },
                    { new Guid("33333333-3333-3333-3333-333333333339"), "Colesterol alto" }
                });

            migrationBuilder.InsertData(
                table: "Gustos",
                columns: new[] { "Id", "ImagenUrl", "Nombre" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, "Pizza" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), null, "Sushi" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), null, "Pastas" },
                    { new Guid("11111111-1111-1111-1111-111111111114"), null, "Milanesa con papas fritas" },
                    { new Guid("11111111-1111-1111-1111-111111111115"), null, "Empanadas" },
                    { new Guid("11111111-1111-1111-1111-111111111116"), null, "Paella" },
                    { new Guid("11111111-1111-1111-1111-111111111117"), null, "Tacos" },
                    { new Guid("11111111-1111-1111-1111-111111111118"), null, "Choripán" },
                    { new Guid("11111111-1111-1111-1111-111111111119"), null, "Risotto" },
                    { new Guid("11111111-1111-1111-1111-111111111120"), null, "Guiso de lentejas" },
                    { new Guid("11111111-1111-1111-1111-111111111121"), null, "Pizza napolitana" },
                    { new Guid("11111111-1111-1111-1111-111111111122"), null, "Ñoquis" },
                    { new Guid("11111111-1111-1111-1111-111111111123"), null, "Ravioles" },
                    { new Guid("11111111-1111-1111-1111-111111111124"), null, "Ensalada César" },
                    { new Guid("11111111-1111-1111-1111-111111111125"), null, "Ramen japonés" },
                    { new Guid("11111111-1111-1111-1111-111111111126"), null, "Tarta de jamón y queso" },
                    { new Guid("11111111-1111-1111-1111-111111111127"), null, "Ceviche peruano" },
                    { new Guid("11111111-1111-1111-1111-111111111128"), null, "Ensaladas" },
                    { new Guid("11111111-1111-1111-1111-111111111129"), null, "Pollo frito" },
                    { new Guid("11111111-1111-1111-1111-111111111130"), null, "Papas fritas" },
                    { new Guid("11111111-1111-1111-1111-111111111131"), null, "Kebab" },
                    { new Guid("11111111-1111-1111-1111-111111111132"), null, "Flan" },
                    { new Guid("11111111-1111-1111-1111-111111111133"), null, "Helado" }
                });

            migrationBuilder.InsertData(
                table: "Restricciones",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222221"), "Lactosa" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Fructosa" },
                    { new Guid("22222222-2222-2222-2222-222222222223"), "Gluten no celíaco" },
                    { new Guid("22222222-2222-2222-2222-222222222224"), "Cafeína" },
                    { new Guid("22222222-2222-2222-2222-222222222225"), "Maní" },
                    { new Guid("22222222-2222-2222-2222-222222222226"), "Pescado" },
                    { new Guid("22222222-2222-2222-2222-222222222227"), "Chocolate" },
                    { new Guid("22222222-2222-2222-2222-222222222228"), "Gluten" },
                    { new Guid("22222222-2222-2222-2222-222222222229"), "Mariscos" },
                    { new Guid("22222222-2222-2222-2222-222222222230"), "Frutos secos" },
                    { new Guid("22222222-2222-2222-2222-222222222231"), "Mostaza" }
                });

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
                name: "IX_UsuarioCondicionesMedicas_UsuariosId",
                table: "UsuarioCondicionesMedicas",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioGustos_UsuariosId",
                table: "UsuarioGustos",
                column: "UsuariosId");

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
                name: "InvitacionesGrupos");

            migrationBuilder.DropTable(
                name: "MiembrosGrupos");

            migrationBuilder.DropTable(
                name: "UsuarioCondicionesMedicas");

            migrationBuilder.DropTable(
                name: "UsuarioGustos");

            migrationBuilder.DropTable(
                name: "UsuarioRestricciones");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "CondicionesMedicas");

            migrationBuilder.DropTable(
                name: "Gustos");

            migrationBuilder.DropTable(
                name: "Restricciones");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
