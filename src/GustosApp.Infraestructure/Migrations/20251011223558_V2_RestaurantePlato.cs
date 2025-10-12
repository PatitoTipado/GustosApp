using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class V2_RestaurantePlato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_Latitud_Longitud",
                table: "Restaurantes");

            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_NombreNormalizado",
                table: "Restaurantes");

            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_PropietarioUid",
                table: "Restaurantes");

            migrationBuilder.AlterColumn<string>(
                name: "PropietarioUid",
                table: "Restaurantes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagenUrl",
                table: "Restaurantes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Restaurantes",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Valoracion",
                table: "Restaurantes",
                type: "decimal(3,2)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "UX_Restaurantes_NombreNormalizado",
                table: "Restaurantes",
                column: "NombreNormalizado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Restaurantes_PropietarioUid",
                table: "Restaurantes",
                column: "PropietarioUid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantePlatos");

            migrationBuilder.DropIndex(
                name: "UX_Restaurantes_NombreNormalizado",
                table: "Restaurantes");

            migrationBuilder.DropIndex(
                name: "UX_Restaurantes_PropietarioUid",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "Valoracion",
                table: "Restaurantes");

            migrationBuilder.AlterColumn<string>(
                name: "PropietarioUid",
                table: "Restaurantes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ImagenUrl",
                table: "Restaurantes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_Latitud_Longitud",
                table: "Restaurantes",
                columns: new[] { "Latitud", "Longitud" });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_NombreNormalizado",
                table: "Restaurantes",
                column: "NombreNormalizado",
                unique: true,
                filter: "[NombreNormalizado] IS NOT NULL AND [NombreNormalizado] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_PropietarioUid",
                table: "Restaurantes",
                column: "PropietarioUid",
                unique: true,
                filter: "[PropietarioUid] IS NOT NULL AND [PropietarioUid] <> ''");
        }
    }
}
