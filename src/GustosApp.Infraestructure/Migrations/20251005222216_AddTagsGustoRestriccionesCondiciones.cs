using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsGustoRestriccionesCondiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PasoActual",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CondicionMedicaTags",
                columns: table => new
                {
                    CondicionMedicaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsCriticosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionMedicaTags", x => new { x.CondicionMedicaId, x.TagsCriticosId });
                    table.ForeignKey(
                        name: "FK_CondicionMedicaTags_CondicionesMedicas_CondicionMedicaId",
                        column: x => x.CondicionMedicaId,
                        principalTable: "CondicionesMedicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CondicionMedicaTags_Tag_TagsCriticosId",
                        column: x => x.TagsCriticosId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GustoTags",
                columns: table => new
                {
                    GustoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GustoTags", x => new { x.GustoId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_GustoTags_Gustos_GustoId",
                        column: x => x.GustoId,
                        principalTable: "Gustos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GustoTags_Tag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestriccionTags",
                columns: table => new
                {
                    RestriccionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsProhibidosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionTags", x => new { x.RestriccionId, x.TagsProhibidosId });
                    table.ForeignKey(
                        name: "FK_RestriccionTags_Restricciones_RestriccionId",
                        column: x => x.RestriccionId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestriccionTags_Tag_TagsProhibidosId",
                        column: x => x.TagsProhibidosId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CondicionMedicaTags_TagsCriticosId",
                table: "CondicionMedicaTags",
                column: "TagsCriticosId");

            migrationBuilder.CreateIndex(
                name: "IX_GustoTags_TagsId",
                table: "GustoTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionTags_TagsProhibidosId",
                table: "RestriccionTags",
                column: "TagsProhibidosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CondicionMedicaTags");

            migrationBuilder.DropTable(
                name: "GustoTags");

            migrationBuilder.DropTable(
                name: "RestriccionTags");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropColumn(
                name: "PasoActual",
                table: "Usuarios");
        }
    }
}
