using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestauranteRelacionesCentralizadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_RestauranteGustos_restaurantesId",
                table: "RestauranteGustos",
                column: "restaurantesId");

            migrationBuilder.CreateIndex(
                name: "IX_RestauranteRestricciones_RestriccionesQueRespetaId",
                table: "RestauranteRestricciones",
                column: "RestriccionesQueRespetaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestauranteGustos");

            migrationBuilder.DropTable(
                name: "RestauranteRestricciones");
        }
    }
}
