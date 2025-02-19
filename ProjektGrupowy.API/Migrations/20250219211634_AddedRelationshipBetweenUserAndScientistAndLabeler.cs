using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedRelationshipBetweenUserAndScientistAndLabeler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Scientists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Labelers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Scientists_UserId",
                table: "Scientists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labelers_UserId",
                table: "Labelers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Labelers_AspNetUsers_UserId",
                table: "Labelers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scientists_AspNetUsers_UserId",
                table: "Scientists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labelers_AspNetUsers_UserId",
                table: "Labelers");

            migrationBuilder.DropForeignKey(
                name: "FK_Scientists_AspNetUsers_UserId",
                table: "Scientists");

            migrationBuilder.DropIndex(
                name: "IX_Scientists_UserId",
                table: "Scientists");

            migrationBuilder.DropIndex(
                name: "IX_Labelers_UserId",
                table: "Labelers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Scientists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Labelers");
        }
    }
}
