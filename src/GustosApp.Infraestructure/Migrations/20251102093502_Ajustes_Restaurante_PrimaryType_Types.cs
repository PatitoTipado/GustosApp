using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Ajustes_Restaurante_PrimaryType_Types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Restaurantes");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryType",
                table: "Restaurantes",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "restaurant");

            migrationBuilder.AddColumn<string>(
                name: "TypesJson",
                table: "Restaurantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_PrimaryType",
                table: "Restaurantes",
                column: "PrimaryType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_PrimaryType",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "PrimaryType",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "TypesJson",
                table: "Restaurantes");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Restaurantes",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }
    }
}
