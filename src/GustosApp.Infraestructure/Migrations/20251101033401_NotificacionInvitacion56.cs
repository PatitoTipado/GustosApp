using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GustosApp.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificacionInvitacion56 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InvitacionesGrupos_NotificacionId",
                table: "InvitacionesGrupos",
                column: "NotificacionId",
                unique: true,
                filter: "[NotificacionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_InvitacionesGrupos_Notificaciones_NotificacionId",
                table: "InvitacionesGrupos",
                column: "NotificacionId",
                principalTable: "Notificaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitacionesGrupos_Notificaciones_NotificacionId",
                table: "InvitacionesGrupos");

            migrationBuilder.DropIndex(
                name: "IX_InvitacionesGrupos_NotificacionId",
                table: "InvitacionesGrupos");
        }
    }
}
