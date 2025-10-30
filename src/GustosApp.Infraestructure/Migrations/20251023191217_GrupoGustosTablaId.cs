using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class GrupoGustosTablaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GrupoGusto",
                table: "GrupoGusto");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GrupoGusto",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_GrupoGusto",
                table: "GrupoGusto",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoGusto_GrupoId",
                table: "GrupoGusto",
                column: "GrupoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GrupoGusto",
                table: "GrupoGusto");

            migrationBuilder.DropIndex(
                name: "IX_GrupoGusto_GrupoId",
                table: "GrupoGusto");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GrupoGusto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GrupoGusto",
                table: "GrupoGusto",
                columns: new[] { "GrupoId", "GustoId" });
        }
    }
}
