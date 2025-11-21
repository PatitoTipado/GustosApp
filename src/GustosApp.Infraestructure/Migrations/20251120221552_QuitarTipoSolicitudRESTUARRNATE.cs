using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class QuitarTipoSolicitudRESTUARRNATE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryType",
                table: "SolicitudesRestaurantes");

            migrationBuilder.DropColumn(
                name: "TypesJson",
                table: "SolicitudesRestaurantes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryType",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypesJson",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
