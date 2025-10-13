using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    public partial class V3_RemoveOwnerUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Restaurantes_PropietarioUid",
                table: "Restaurantes");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_PropietarioUid",
                table: "Restaurantes",
                column: "PropietarioUid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
