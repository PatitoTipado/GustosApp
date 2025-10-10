using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial_Restaurantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Restaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropietarioUid = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    NombreNormalizado = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Latitud = table.Column<double>(type: "float", nullable: false),
                    Longitud = table.Column<double>(type: "float", nullable: false),
                    HorariosJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurantes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_Latitud_Longitud",
                table: "Restaurantes",
                columns: new[] { "Latitud", "Longitud" });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_NombreNormalizado",
                table: "Restaurantes",
                column: "NombreNormalizado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_PropietarioUid",
                table: "Restaurantes",
                column: "PropietarioUid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Restaurantes");
        }
    }
}
