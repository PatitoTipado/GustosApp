using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_Ahora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Restaurantes_PropietarioUid",
                table: "Restaurantes");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_Latitud_Longitud",
                table: "Restaurantes",
                columns: new[] { "Latitud", "Longitud" });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_PropietarioUid",
                table: "Restaurantes",
                column: "PropietarioUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_Latitud_Longitud",
                table: "Restaurantes");

            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_PropietarioUid",
                table: "Restaurantes");

            migrationBuilder.CreateIndex(
                name: "UX_Restaurantes_PropietarioUid",
                table: "Restaurantes",
                column: "PropietarioUid",
                unique: true);
        }
    }
}
