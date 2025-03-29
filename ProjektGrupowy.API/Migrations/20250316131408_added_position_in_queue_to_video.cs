using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class added_position_in_queue_to_video : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vidoes_Projects_ProjectId",
                table: "Vidoes");

            migrationBuilder.DropIndex(
                name: "IX_Vidoes_ProjectId",
                table: "Vidoes");

            migrationBuilder.DropIndex(
                name: "IX_Vidoes_VideoGroupId",
                table: "Vidoes");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Vidoes");

            migrationBuilder.AddColumn<int>(
                name: "PositionInQueue",
                table: "Vidoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vidoes_VideoGroupId_PositionInQueue",
                table: "Vidoes",
                columns: new[] { "VideoGroupId", "PositionInQueue" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vidoes_VideoGroupId_PositionInQueue",
                table: "Vidoes");

            migrationBuilder.DropColumn(
                name: "PositionInQueue",
                table: "Vidoes");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Vidoes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vidoes_ProjectId",
                table: "Vidoes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Vidoes_VideoGroupId",
                table: "Vidoes",
                column: "VideoGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vidoes_Projects_ProjectId",
                table: "Vidoes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
