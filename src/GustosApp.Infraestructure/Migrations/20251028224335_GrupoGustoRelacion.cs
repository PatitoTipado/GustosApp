using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class GrupoGustoRelacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrupoGusto_Grupos_GrupoId",
                table: "GrupoGusto");

            migrationBuilder.DropForeignKey(
                name: "FK_GrupoGusto_Gustos_GustoId",
                table: "GrupoGusto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GrupoGusto",
                table: "GrupoGusto");

            migrationBuilder.RenameTable(
                name: "GrupoGusto",
                newName: "GrupoGustos");

            migrationBuilder.RenameIndex(
                name: "IX_GrupoGusto_GustoId",
                table: "GrupoGustos",
                newName: "IX_GrupoGustos_GustoId");

            migrationBuilder.RenameIndex(
                name: "IX_GrupoGusto_GrupoId",
                table: "GrupoGustos",
                newName: "IX_GrupoGustos_GrupoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GrupoGustos",
                table: "GrupoGustos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoGustos_Grupos_GrupoId",
                table: "GrupoGustos",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoGustos_Gustos_GustoId",
                table: "GrupoGustos",
                column: "GustoId",
                principalTable: "Gustos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrupoGustos_Grupos_GrupoId",
                table: "GrupoGustos");

            migrationBuilder.DropForeignKey(
                name: "FK_GrupoGustos_Gustos_GustoId",
                table: "GrupoGustos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GrupoGustos",
                table: "GrupoGustos");

            migrationBuilder.RenameTable(
                name: "GrupoGustos",
                newName: "GrupoGusto");

            migrationBuilder.RenameIndex(
                name: "IX_GrupoGustos_GustoId",
                table: "GrupoGusto",
                newName: "IX_GrupoGusto_GustoId");

            migrationBuilder.RenameIndex(
                name: "IX_GrupoGustos_GrupoId",
                table: "GrupoGusto",
                newName: "IX_GrupoGusto_GrupoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GrupoGusto",
                table: "GrupoGusto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoGusto_Grupos_GrupoId",
                table: "GrupoGusto",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoGusto_Gustos_GustoId",
                table: "GrupoGusto",
                column: "GustoId",
                principalTable: "Gustos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
