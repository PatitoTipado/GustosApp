using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class CambioNombreReseña : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewsRestaurantes_Restaurantes_RestauranteId",
                table: "ReviewsRestaurantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewsRestaurantes",
                table: "ReviewsRestaurantes");

            migrationBuilder.RenameTable(
                name: "ReviewsRestaurantes",
                newName: "ReseñasRestaurantes");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewsRestaurantes_RestauranteId",
                table: "ReseñasRestaurantes",
                newName: "IX_ReseñasRestaurantes_RestauranteId");

            migrationBuilder.AddColumn<string>(
                name: "Fecha",
                table: "ReseñasRestaurantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "ReseñasRestaurantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReseñasRestaurantes",
                table: "ReseñasRestaurantes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReseñasRestaurantes_Restaurantes_RestauranteId",
                table: "ReseñasRestaurantes",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReseñasRestaurantes_Restaurantes_RestauranteId",
                table: "ReseñasRestaurantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReseñasRestaurantes",
                table: "ReseñasRestaurantes");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "ReseñasRestaurantes");

            migrationBuilder.DropColumn(
                name: "Foto",
                table: "ReseñasRestaurantes");

            migrationBuilder.RenameTable(
                name: "ReseñasRestaurantes",
                newName: "ReviewsRestaurantes");

            migrationBuilder.RenameIndex(
                name: "IX_ReseñasRestaurantes_RestauranteId",
                table: "ReviewsRestaurantes",
                newName: "IX_ReviewsRestaurantes_RestauranteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewsRestaurantes",
                table: "ReviewsRestaurantes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewsRestaurantes_Restaurantes_RestauranteId",
                table: "ReviewsRestaurantes",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
