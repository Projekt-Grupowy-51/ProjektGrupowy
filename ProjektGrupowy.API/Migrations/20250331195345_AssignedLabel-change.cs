using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class AssignedLabelchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedLabels_SubjectVideoGroupAssignments_SubjectVideoGro~",
                table: "AssignedLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_Vidoes_VideoGroups_VideoGroupId",
                table: "Vidoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vidoes",
                table: "Vidoes");

            migrationBuilder.RenameTable(
                name: "Vidoes",
                newName: "Videos");

            migrationBuilder.RenameColumn(
                name: "SubjectVideoGroupAssignmentId",
                table: "AssignedLabels",
                newName: "VideoId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedLabels_SubjectVideoGroupAssignmentId",
                table: "AssignedLabels",
                newName: "IX_AssignedLabels_VideoId");

            migrationBuilder.RenameIndex(
                name: "IX_Vidoes_VideoGroupId_PositionInQueue",
                table: "Videos",
                newName: "IX_Videos_VideoGroupId_PositionInQueue");

            migrationBuilder.AlterColumn<string>(
                name: "Start",
                table: "AssignedLabels",
                type: "text",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<string>(
                name: "End",
                table: "AssignedLabels",
                type: "text",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Videos",
                table: "Videos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedLabels_Videos_VideoId",
                table: "AssignedLabels",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_VideoGroups_VideoGroupId",
                table: "Videos",
                column: "VideoGroupId",
                principalTable: "VideoGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedLabels_Videos_VideoId",
                table: "AssignedLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_VideoGroups_VideoGroupId",
                table: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Videos",
                table: "Videos");

            migrationBuilder.RenameTable(
                name: "Videos",
                newName: "Vidoes");

            migrationBuilder.RenameColumn(
                name: "VideoId",
                table: "AssignedLabels",
                newName: "SubjectVideoGroupAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedLabels_VideoId",
                table: "AssignedLabels",
                newName: "IX_AssignedLabels_SubjectVideoGroupAssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Videos_VideoGroupId_PositionInQueue",
                table: "Vidoes",
                newName: "IX_Vidoes_VideoGroupId_PositionInQueue");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Start",
                table: "AssignedLabels",
                type: "interval",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "End",
                table: "AssignedLabels",
                type: "interval",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vidoes",
                table: "Vidoes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedLabels_SubjectVideoGroupAssignments_SubjectVideoGro~",
                table: "AssignedLabels",
                column: "SubjectVideoGroupAssignmentId",
                principalTable: "SubjectVideoGroupAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vidoes_VideoGroups_VideoGroupId",
                table: "Vidoes",
                column: "VideoGroupId",
                principalTable: "VideoGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
