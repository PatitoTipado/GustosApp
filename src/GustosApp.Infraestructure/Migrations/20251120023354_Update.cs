using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NombreRestaurante",
                table: "SolicitudesRestaurantes",
                newName: "TypesJson");

            migrationBuilder.RenameColumn(
                name: "FechaSolicitud",
                table: "SolicitudesRestaurantes",
                newName: "Fecha");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "SolicitudesRestaurantes",
                newName: "RestriccionesIds");

            migrationBuilder.AddColumn<string>(
                name: "GustosIds",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "HorariosJson",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitud",
                table: "SolicitudesRestaurantes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitud",
                table: "SolicitudesRestaurantes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Platos",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryType",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GustosIds",
                table: "SolicitudesRestaurantes");

            migrationBuilder.DropColumn(
                name: "HorariosJson",
                table: "SolicitudesRestaurantes");

            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "SolicitudesRestaurantes");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "SolicitudesRestaurantes");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "SolicitudesRestaurantes");

            migrationBuilder.DropColumn(
                name: "Platos",
                table: "SolicitudesRestaurantes");

            migrationBuilder.DropColumn(
                name: "PrimaryType",
                table: "SolicitudesRestaurantes");

            migrationBuilder.RenameColumn(
                name: "TypesJson",
                table: "SolicitudesRestaurantes",
                newName: "NombreRestaurante");

            migrationBuilder.RenameColumn(
                name: "RestriccionesIds",
                table: "SolicitudesRestaurantes",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "SolicitudesRestaurantes",
                newName: "FechaSolicitud");
        }
    }
}
