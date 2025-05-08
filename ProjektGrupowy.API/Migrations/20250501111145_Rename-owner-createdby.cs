using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class Renameownercreatedby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedLabels_AspNetUsers_OwnerId",
                table: "AssignedLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneratedReports_AspNetUsers_OwnerId",
                table: "GeneratedReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_AspNetUsers_OwnerId",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAccessCodes_AspNetUsers_OwnerId",
                table: "ProjectAccessCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_AspNetUsers_OwnerId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectVideoGroupAssignments_AspNetUsers_OwnerId",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoGroups_AspNetUsers_OwnerId",
                table: "VideoGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_AspNetUsers_OwnerId",
                table: "Videos");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Videos",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Videos_OwnerId",
                table: "Videos",
                newName: "IX_Videos_CreatedById");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "VideoGroups",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_VideoGroups_OwnerId",
                table: "VideoGroups",
                newName: "IX_VideoGroups_CreatedById");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "SubjectVideoGroupAssignments",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectVideoGroupAssignments_OwnerId",
                table: "SubjectVideoGroupAssignments",
                newName: "IX_SubjectVideoGroupAssignments_CreatedById");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Subjects",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_OwnerId",
                table: "Subjects",
                newName: "IX_Subjects_CreatedById");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Projects",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                newName: "IX_Projects_CreatedById");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "ProjectAccessCodes",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectAccessCodes_OwnerId",
                table: "ProjectAccessCodes",
                newName: "IX_ProjectAccessCodes_CreatedById");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Labels",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Labels_OwnerId",
                table: "Labels",
                newName: "IX_Labels_CreatedById");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "GeneratedReports",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_GeneratedReports_OwnerId",
                table: "GeneratedReports",
                newName: "IX_GeneratedReports_CreatedById");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "AssignedLabels",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedLabels_OwnerId",
                table: "AssignedLabels",
                newName: "IX_AssignedLabels_CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedLabels_AspNetUsers_CreatedById",
                table: "AssignedLabels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneratedReports_AspNetUsers_CreatedById",
                table: "GeneratedReports",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_AspNetUsers_CreatedById",
                table: "Labels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAccessCodes_AspNetUsers_CreatedById",
                table: "ProjectAccessCodes",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedById",
                table: "Projects",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_AspNetUsers_CreatedById",
                table: "Subjects",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectVideoGroupAssignments_AspNetUsers_CreatedById",
                table: "SubjectVideoGroupAssignments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoGroups_AspNetUsers_CreatedById",
                table: "VideoGroups",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_AspNetUsers_CreatedById",
                table: "Videos",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedLabels_AspNetUsers_CreatedById",
                table: "AssignedLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneratedReports_AspNetUsers_CreatedById",
                table: "GeneratedReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_AspNetUsers_CreatedById",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAccessCodes_AspNetUsers_CreatedById",
                table: "ProjectAccessCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_AspNetUsers_CreatedById",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectVideoGroupAssignments_AspNetUsers_CreatedById",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoGroups_AspNetUsers_CreatedById",
                table: "VideoGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_AspNetUsers_CreatedById",
                table: "Videos");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Videos",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Videos_CreatedById",
                table: "Videos",
                newName: "IX_Videos_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "VideoGroups",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_VideoGroups_CreatedById",
                table: "VideoGroups",
                newName: "IX_VideoGroups_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "SubjectVideoGroupAssignments",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectVideoGroupAssignments_CreatedById",
                table: "SubjectVideoGroupAssignments",
                newName: "IX_SubjectVideoGroupAssignments_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Subjects",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_CreatedById",
                table: "Subjects",
                newName: "IX_Subjects_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Projects",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                newName: "IX_Projects_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "ProjectAccessCodes",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectAccessCodes_CreatedById",
                table: "ProjectAccessCodes",
                newName: "IX_ProjectAccessCodes_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Labels",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Labels_CreatedById",
                table: "Labels",
                newName: "IX_Labels_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "GeneratedReports",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_GeneratedReports_CreatedById",
                table: "GeneratedReports",
                newName: "IX_GeneratedReports_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "AssignedLabels",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedLabels_CreatedById",
                table: "AssignedLabels",
                newName: "IX_AssignedLabels_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedLabels_AspNetUsers_OwnerId",
                table: "AssignedLabels",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneratedReports_AspNetUsers_OwnerId",
                table: "GeneratedReports",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_AspNetUsers_OwnerId",
                table: "Labels",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAccessCodes_AspNetUsers_OwnerId",
                table: "ProjectAccessCodes",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_AspNetUsers_OwnerId",
                table: "Subjects",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectVideoGroupAssignments_AspNetUsers_OwnerId",
                table: "SubjectVideoGroupAssignments",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoGroups_AspNetUsers_OwnerId",
                table: "VideoGroups",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_AspNetUsers_OwnerId",
                table: "Videos",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
