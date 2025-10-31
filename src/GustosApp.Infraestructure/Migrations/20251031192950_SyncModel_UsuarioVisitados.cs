using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_UsuarioVisitados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsPrivado",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioRestauranteVisitados");

            migrationBuilder.DropColumn(
                name: "EsPrivado",
                table: "Usuarios");
        }
    }
}
