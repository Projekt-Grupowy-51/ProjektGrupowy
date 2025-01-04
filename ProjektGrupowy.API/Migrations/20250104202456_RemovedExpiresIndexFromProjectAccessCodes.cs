using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class RemovedExpiresIndexFromProjectAccessCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectAccessCode_ExpiresAtUtc",
                table: "ProjectAccessCodes");

            // Create a compound index on (ProjectId, ExpiresAtUtc)
            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccessCodes_ProjectId_ExpiresAtUtc",
                table: "ProjectAccessCodes",
                columns: ["ProjectId", "ExpiresAtUtc"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccessCode_ExpiresAtUtc",
                table: "ProjectAccessCodes",
                column: "ExpiresAtUtc");

            // Remove the compound index
            migrationBuilder.DropIndex(
                name: "IX_ProjectAccessCodes_ProjectId_ExpiresAtUtc",
                table: "ProjectAccessCodes");
        }
    }
}
