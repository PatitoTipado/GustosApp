using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class CambiosEnClases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_GrupoId",
                table: "ChatMessages",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UsuarioId",
                table: "ChatMessages",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Grupos_GrupoId",
                table: "ChatMessages",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Usuarios_UsuarioId",
                table: "ChatMessages",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Grupos_GrupoId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Usuarios_UsuarioId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_GrupoId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_UsuarioId",
                table: "ChatMessages");
        }
    }
}
