using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionesFotos_OpinionesRestaurantes_OpinionRestauranteId",
                table: "OpinionesFotos");

            migrationBuilder.DropForeignKey(
                name: "FK_OpinionesRestaurantes_Restaurantes_RestauranteId",
                table: "OpinionesRestaurantes");

            migrationBuilder.DropForeignKey(
                name: "FK_OpinionesRestaurantes_Usuarios_UsuarioId",
                table: "OpinionesRestaurantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpinionesRestaurantes",
                table: "OpinionesRestaurantes");

            migrationBuilder.RenameTable(
                name: "OpinionesRestaurantes",
                newName: "OpinionRestaurante");

            migrationBuilder.RenameIndex(
                name: "IX_OpinionesRestaurantes_UsuarioId",
                table: "OpinionRestaurante",
                newName: "IX_OpinionRestaurante_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_OpinionesRestaurantes_RestauranteId",
                table: "OpinionRestaurante",
                newName: "IX_OpinionRestaurante_RestauranteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpinionRestaurante",
                table: "OpinionRestaurante",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionesFotos_OpinionRestaurante_OpinionRestauranteId",
                table: "OpinionesFotos",
                column: "OpinionRestauranteId",
                principalTable: "OpinionRestaurante",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpinionesFotos_OpinionRestaurante_OpinionRestauranteId",
                table: "OpinionesFotos");

            migrationBuilder.DropForeignKey(
                name: "FK_OpinionRestaurante_Restaurantes_RestauranteId",
                table: "OpinionRestaurante");

            migrationBuilder.DropForeignKey(
                name: "FK_OpinionRestaurante_Usuarios_UsuarioId",
                table: "OpinionRestaurante");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OpinionRestaurante",
                table: "OpinionRestaurante");

            migrationBuilder.RenameTable(
                name: "OpinionRestaurante",
                newName: "OpinionesRestaurantes");

            migrationBuilder.RenameIndex(
                name: "IX_OpinionRestaurante_UsuarioId",
                table: "OpinionesRestaurantes",
                newName: "IX_OpinionesRestaurantes_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_OpinionRestaurante_RestauranteId",
                table: "OpinionesRestaurantes",
                newName: "IX_OpinionesRestaurantes_RestauranteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OpinionesRestaurantes",
                table: "OpinionesRestaurantes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OpinionesFotos_OpinionesRestaurantes_OpinionRestauranteId",
                table: "OpinionesFotos",
                column: "OpinionRestauranteId",
                principalTable: "OpinionesRestaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
