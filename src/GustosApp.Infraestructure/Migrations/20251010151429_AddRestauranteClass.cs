using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestauranteClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CondicionMedicaTags_Tag_TagsCriticosId",
                table: "CondicionMedicaTags");

            migrationBuilder.DropForeignKey(
                name: "FK_GustoTags_Tag_TagsId",
                table: "GustoTags");

            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionTags_Tag_TagsProhibidosId",
                table: "RestriccionTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Restaurantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitud = table.Column<double>(type: "float", nullable: false),
                    Longitud = table.Column<double>(type: "float", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    CantidadResenas = table.Column<int>(type: "int", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurantes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CondicionMedicaTags_Tags_TagsCriticosId",
                table: "CondicionMedicaTags",
                column: "TagsCriticosId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GustoTags_Tags_TagsId",
                table: "GustoTags",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionTags_Tags_TagsProhibidosId",
                table: "RestriccionTags",
                column: "TagsProhibidosId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CondicionMedicaTags_Tags_TagsCriticosId",
                table: "CondicionMedicaTags");

            migrationBuilder.DropForeignKey(
                name: "FK_GustoTags_Tags_TagsId",
                table: "GustoTags");

            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionTags_Tags_TagsProhibidosId",
                table: "RestriccionTags");

            migrationBuilder.DropTable(
                name: "Restaurantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CondicionMedicaTags_Tag_TagsCriticosId",
                table: "CondicionMedicaTags",
                column: "TagsCriticosId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GustoTags_Tag_TagsId",
                table: "GustoTags",
                column: "TagsId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionTags_Tag_TagsProhibidosId",
                table: "RestriccionTags",
                column: "TagsProhibidosId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
