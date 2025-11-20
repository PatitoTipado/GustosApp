using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionTablasYAgregadoDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudRestauranteImagen_SolicitudesRestaurantes_SolicitudId",
                table: "SolicitudRestauranteImagen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SolicitudRestauranteImagen",
                table: "SolicitudRestauranteImagen");

            migrationBuilder.DropColumn(
                name: "Platos",
                table: "SolicitudesRestaurantes");

            migrationBuilder.RenameTable(
                name: "SolicitudRestauranteImagen",
                newName: "SolicitudRestauranteImagenes");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "SolicitudesRestaurantes",
                newName: "FechaCreacion");

            migrationBuilder.RenameIndex(
                name: "IX_SolicitudRestauranteImagen_SolicitudId",
                table: "SolicitudRestauranteImagenes",
                newName: "IX_SolicitudRestauranteImagenes_SolicitudId");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryType",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Restaurantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MenuError",
                table: "Restaurantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MenuProcesado",
                table: "Restaurantes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SolicitudRestauranteImagenes",
                table: "SolicitudRestauranteImagenes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SolicitudRestaurante_Gustos",
                columns: table => new
                {
                    GustosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudRestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudRestaurante_Gustos", x => new { x.GustosId, x.SolicitudRestauranteId });
                    table.ForeignKey(
                        name: "FK_SolicitudRestaurante_Gustos_Gustos_GustosId",
                        column: x => x.GustosId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudRestaurante_Gustos_SolicitudesRestaurantes_SolicitudRestauranteId",
                        column: x => x.SolicitudRestauranteId,
                        principalTable: "SolicitudesRestaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudRestaurante_Restricciones",
                columns: table => new
                {
                    RestriccionesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudRestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudRestaurante_Restricciones", x => new { x.RestriccionesId, x.SolicitudRestauranteId });
                    table.ForeignKey(
                        name: "FK_SolicitudRestaurante_Restricciones_Restricciones_RestriccionesId",
                        column: x => x.RestriccionesId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudRestaurante_Restricciones_SolicitudesRestaurantes_SolicitudRestauranteId",
                        column: x => x.SolicitudRestauranteId,
                        principalTable: "SolicitudesRestaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRestaurantes_Estado",
                table: "SolicitudesRestaurantes",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudRestaurante_Gustos_SolicitudRestauranteId",
                table: "SolicitudRestaurante_Gustos",
                column: "SolicitudRestauranteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudRestaurante_Restricciones_SolicitudRestauranteId",
                table: "SolicitudRestaurante_Restricciones",
                column: "SolicitudRestauranteId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudRestauranteImagenes_SolicitudesRestaurantes_SolicitudId",
                table: "SolicitudRestauranteImagenes",
                column: "SolicitudId",
                principalTable: "SolicitudesRestaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudRestauranteImagenes_SolicitudesRestaurantes_SolicitudId",
                table: "SolicitudRestauranteImagenes");

            migrationBuilder.DropTable(
                name: "SolicitudRestaurante_Gustos");

            migrationBuilder.DropTable(
                name: "SolicitudRestaurante_Restricciones");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesRestaurantes_Estado",
                table: "SolicitudesRestaurantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SolicitudRestauranteImagenes",
                table: "SolicitudRestauranteImagenes");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "MenuError",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "MenuProcesado",
                table: "Restaurantes");

            migrationBuilder.RenameTable(
                name: "SolicitudRestauranteImagenes",
                newName: "SolicitudRestauranteImagen");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "SolicitudesRestaurantes",
                newName: "Fecha");

            migrationBuilder.RenameIndex(
                name: "IX_SolicitudRestauranteImagenes_SolicitudId",
                table: "SolicitudRestauranteImagen",
                newName: "IX_SolicitudRestauranteImagen_SolicitudId");

            migrationBuilder.AlterColumn<string>(
                name: "PrimaryType",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "Platos",
                table: "SolicitudesRestaurantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SolicitudRestauranteImagen",
                table: "SolicitudRestauranteImagen",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudRestauranteImagen_SolicitudesRestaurantes_SolicitudId",
                table: "SolicitudRestauranteImagen",
                column: "SolicitudId",
                principalTable: "SolicitudesRestaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
