using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_NombreNormalizado_NonUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_PropietarioUid",
                table: "Restaurantes");

            migrationBuilder.DropIndex(
                name: "UX_Restaurantes_NombreNormalizado",
                table: "Restaurantes");

            migrationBuilder.AlterColumn<string>(
                name: "WebUrl",
                table: "Restaurantes",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlaceId",
                table: "Restaurantes",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Restaurantes",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_NombreNormalizado",
                table: "Restaurantes",
                column: "NombreNormalizado");

            migrationBuilder.CreateIndex(
                name: "UX_Restaurantes_PlaceId",
                table: "Restaurantes",
                column: "PlaceId",
                unique: true,
                filter: "[PlaceId] IS NOT NULL AND [PlaceId] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_NombreNormalizado",
                table: "Restaurantes");

            migrationBuilder.DropIndex(
                name: "UX_Restaurantes_PlaceId",
                table: "Restaurantes");

            migrationBuilder.AlterColumn<string>(
                name: "WebUrl",
                table: "Restaurantes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlaceId",
                table: "Restaurantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "Restaurantes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_PropietarioUid",
                table: "Restaurantes",
                column: "PropietarioUid");

            migrationBuilder.CreateIndex(
                name: "UX_Restaurantes_NombreNormalizado",
                table: "Restaurantes",
                column: "NombreNormalizado",
                unique: true);
        }
    }
}
