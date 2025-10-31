using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValoracionesUsuarios_Restaurantes_RestauranteId",
                table: "ValoracionesUsuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_ValoracionesUsuarios_Usuarios_UsuarioId",
                table: "ValoracionesUsuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValoracionesUsuarios",
                table: "ValoracionesUsuarios");

            migrationBuilder.RenameTable(
                name: "ValoracionesUsuarios",
                newName: "ValoracionUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ValoracionesUsuarios_UsuarioId_RestauranteId",
                table: "ValoracionUsuario",
                newName: "IX_ValoracionUsuario_UsuarioId_RestauranteId");

            migrationBuilder.RenameIndex(
                name: "IX_ValoracionesUsuarios_RestauranteId",
                table: "ValoracionUsuario",
                newName: "IX_ValoracionUsuario_RestauranteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValoracionUsuario",
                table: "ValoracionUsuario",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ValoracionUsuario_Restaurantes_RestauranteId",
                table: "ValoracionUsuario",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ValoracionUsuario_Usuarios_UsuarioId",
                table: "ValoracionUsuario",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValoracionUsuario_Restaurantes_RestauranteId",
                table: "ValoracionUsuario");

            migrationBuilder.DropForeignKey(
                name: "FK_ValoracionUsuario_Usuarios_UsuarioId",
                table: "ValoracionUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValoracionUsuario",
                table: "ValoracionUsuario");

            migrationBuilder.RenameTable(
                name: "ValoracionUsuario",
                newName: "ValoracionesUsuarios");

            migrationBuilder.RenameIndex(
                name: "IX_ValoracionUsuario_UsuarioId_RestauranteId",
                table: "ValoracionesUsuarios",
                newName: "IX_ValoracionesUsuarios_UsuarioId_RestauranteId");

            migrationBuilder.RenameIndex(
                name: "IX_ValoracionUsuario_RestauranteId",
                table: "ValoracionesUsuarios",
                newName: "IX_ValoracionesUsuarios_RestauranteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValoracionesUsuarios",
                table: "ValoracionesUsuarios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ValoracionesUsuarios_Restaurantes_RestauranteId",
                table: "ValoracionesUsuarios",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ValoracionesUsuarios_Usuarios_UsuarioId",
                table: "ValoracionesUsuarios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
