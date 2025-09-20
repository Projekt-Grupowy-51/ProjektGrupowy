using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_notifications_table_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_user_entity_UserId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Notifications",
                newName: "RecipientId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                newName: "IX_Notifications_RecipientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_user_entity_RecipientId",
                table: "Notifications",
                column: "RecipientId",
                principalTable: "user_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_user_entity_RecipientId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Notifications",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_RecipientId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_user_entity_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "user_entity",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
