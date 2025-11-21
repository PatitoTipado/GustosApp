using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class RolesUsuarioSolicitudRestauranteImagenesRestaurante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rol",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "DuenoId",
                table: "Restaurantes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SolicitudesRestaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreRestaurante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesRestaurantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesRestaurantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudRestauranteImagen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudRestauranteImagen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudRestauranteImagen_SolicitudesRestaurantes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesRestaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurantes_DuenoId",
                table: "Restaurantes",
                column: "DuenoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRestaurantes_UsuarioId",
                table: "SolicitudesRestaurantes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudRestauranteImagen_SolicitudId",
                table: "SolicitudRestauranteImagen",
                column: "SolicitudId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurantes_Usuarios_DuenoId",
                table: "Restaurantes",
                column: "DuenoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurantes_Usuarios_DuenoId",
                table: "Restaurantes");

            migrationBuilder.DropTable(
                name: "SolicitudRestauranteImagen");

            migrationBuilder.DropTable(
                name: "SolicitudesRestaurantes");

            migrationBuilder.DropIndex(
                name: "IX_Restaurantes_DuenoId",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DuenoId",
                table: "Restaurantes");
        }
    }
}
