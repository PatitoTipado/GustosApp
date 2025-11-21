using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class RestauranteEstadisticas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestauranteEstadisticas");
        }
    }
}
