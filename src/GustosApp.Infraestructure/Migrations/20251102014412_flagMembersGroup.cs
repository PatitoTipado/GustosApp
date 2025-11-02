using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class flagMembersGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "afectarRecomendacion",
                table: "MiembrosGrupos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "MiembroId",
                table: "GrupoGustos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GrupoGustos_MiembroId",
                table: "GrupoGustos",
                column: "MiembroId");

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoGustos_MiembrosGrupos_MiembroId",
                table: "GrupoGustos",
                column: "MiembroId",
                principalTable: "MiembrosGrupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrupoGustos_MiembrosGrupos_MiembroId",
                table: "GrupoGustos");

            migrationBuilder.DropIndex(
                name: "IX_GrupoGustos_MiembroId",
                table: "GrupoGustos");

            migrationBuilder.DropColumn(
                name: "afectarRecomendacion",
                table: "MiembrosGrupos");

            migrationBuilder.DropColumn(
                name: "MiembroId",
                table: "GrupoGustos");
        }
    }
}
