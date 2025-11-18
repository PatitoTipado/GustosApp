using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionOpinion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionRestaurante_Restaurantes_RestauranteId",
                table: "OpinionRestaurante");

            migrationBuilder.DropForeignKey(
                name: "FK_OpinionRestaurante_Usuarios_UsuarioId",
                table: "OpinionRestaurante");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpinionRestaurante",
                table: "OpinionRestaurante");

            migrationBuilder.DropIndex(
                name: "IX_OpinionRestaurante_UsuarioId_RestauranteId",
                table: "OpinionRestaurante");

            migrationBuilder.RenameTable(
                name: "OpinionRestaurante",
                newName: "OpinionesRestaurantes");

            migrationBuilder.RenameColumn(
                name: "Img",
                table: "OpinionesRestaurantes",
                newName: "MotivoVisita");

            migrationBuilder.RenameColumn(
                name: "FechaTexto",
                table: "OpinionesRestaurantes",
                newName: "MesAnioVisita");

            migrationBuilder.RenameColumn(
                name: "Autor",
                table: "OpinionesRestaurantes",
                newName: "ImagenAutorExterno");

            migrationBuilder.RenameIndex(
                name: "IX_OpinionRestaurante_RestauranteId",
                table: "OpinionesRestaurantes",
                newName: "IX_OpinionesRestaurantes_RestauranteId");

            migrationBuilder.AlterColumn<double>(
                name: "Valoracion",
                table: "OpinionesRestaurantes",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "OpinionesRestaurantes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "OpinionesRestaurantes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Opinion",
                table: "OpinionesRestaurantes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVisita",
                table: "OpinionesRestaurantes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "AutorExterno",
                table: "OpinionesRestaurantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsImportada",
                table: "OpinionesRestaurantes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FuenteExterna",
                table: "OpinionesRestaurantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpinionesRestaurantes",
                table: "OpinionesRestaurantes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OpinionesFotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpinionRestauranteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpinionesFotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpinionesFotos_OpinionesRestaurantes_OpinionRestauranteId",
                        column: x => x.OpinionRestauranteId,
                        principalTable: "OpinionesRestaurantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpinionesRestaurantes_UsuarioId",
                table: "OpinionesRestaurantes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OpinionesFotos_OpinionRestauranteId",
                table: "OpinionesFotos",
                column: "OpinionRestauranteId");

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionesRestaurantes_Restaurantes_RestauranteId",
                table: "OpinionesRestaurantes",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionesRestaurantes_Usuarios_UsuarioId",
                table: "OpinionesRestaurantes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionesRestaurantes_Restaurantes_RestauranteId",
                table: "OpinionesRestaurantes");

            migrationBuilder.DropForeignKey(
                name: "FK_OpinionesRestaurantes_Usuarios_UsuarioId",
                table: "OpinionesRestaurantes");

            migrationBuilder.DropTable(
                name: "OpinionesFotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpinionesRestaurantes",
                table: "OpinionesRestaurantes");

            migrationBuilder.DropIndex(
                name: "IX_OpinionesRestaurantes_UsuarioId",
                table: "OpinionesRestaurantes");

            migrationBuilder.DropColumn(
                name: "AutorExterno",
                table: "OpinionesRestaurantes");

            migrationBuilder.DropColumn(
                name: "EsImportada",
                table: "OpinionesRestaurantes");

            migrationBuilder.DropColumn(
                name: "FuenteExterna",
                table: "OpinionesRestaurantes");

            migrationBuilder.RenameTable(
                name: "OpinionesRestaurantes",
                newName: "OpinionRestaurante");

            migrationBuilder.RenameColumn(
                name: "MotivoVisita",
                table: "OpinionRestaurante",
                newName: "Img");

            migrationBuilder.RenameColumn(
                name: "MesAnioVisita",
                table: "OpinionRestaurante",
                newName: "FechaTexto");

            migrationBuilder.RenameColumn(
                name: "ImagenAutorExterno",
                table: "OpinionRestaurante",
                newName: "Autor");

            migrationBuilder.RenameIndex(
                name: "IX_OpinionesRestaurantes_RestauranteId",
                table: "OpinionRestaurante",
                newName: "IX_OpinionRestaurante_RestauranteId");

            migrationBuilder.AlterColumn<int>(
                name: "Valoracion",
                table: "OpinionRestaurante",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "OpinionRestaurante",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "OpinionRestaurante",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Opinion",
                table: "OpinionRestaurante",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVisita",
                table: "OpinionRestaurante",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpinionRestaurante",
                table: "OpinionRestaurante",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OpinionRestaurante_UsuarioId_RestauranteId",
                table: "OpinionRestaurante",
                columns: new[] { "UsuarioId", "RestauranteId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionRestaurante_Restaurantes_RestauranteId",
                table: "OpinionRestaurante",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionRestaurante_Usuarios_UsuarioId",
                table: "OpinionRestaurante",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
