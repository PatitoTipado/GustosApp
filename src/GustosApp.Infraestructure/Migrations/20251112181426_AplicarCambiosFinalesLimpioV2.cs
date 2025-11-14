using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AplicarCambiosFinalesLimpioV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropTable(
                name: "ReseñasRestaurantes");

            migrationBuilder.DropTable(
                name: "ValoracionUsuario");
            */
            migrationBuilder.CreateTable(
                name: "OpinionRestaurante",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Valoracion = table.Column<int>(type: "int", nullable: false),
                    Opinion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaVisita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaTexto = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpinionRestaurante_RestauranteId",
                table: "OpinionRestaurante",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_OpinionRestaurante_UsuarioId_RestauranteId",
                table: "OpinionRestaurante",
                columns: new[] { "UsuarioId", "RestauranteId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpinionRestaurante");

            migrationBuilder.CreateTable(
                name: "ReseñasRestaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "ValoracionUsuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valoracion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValoracionUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValoracionUsuario_Restaurantes_RestauranteId",
                        column: x => x.RestauranteId,
                        principalTable: "Restaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValoracionUsuario_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReseñasRestaurantes_RestauranteId",
                table: "ReseñasRestaurantes",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionUsuario_RestauranteId",
                table: "ValoracionUsuario",
                column: "RestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_ValoracionUsuario_UsuarioId_RestauranteId",
                table: "ValoracionUsuario",
                columns: new[] { "UsuarioId", "RestauranteId" },
                unique: true);
        }
    }
}
