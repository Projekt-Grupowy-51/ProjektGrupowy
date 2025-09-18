using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_qf_and_indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_CreatedById",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_VideoGroups_CreatedById",
                table: "VideoGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectVideoGroupAssignments_CreatedById",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_CreatedById",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAccessCodes_CreatedById",
                table: "ProjectAccessCodes");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CreatedById",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_GeneratedReports_CreatedById",
                table: "GeneratedReports");

            migrationBuilder.DropIndex(
                name: "IX_AssignedLabels_CreatedById",
                table: "AssignedLabels");

            migrationBuilder.CreateIndex(
                name: "ix_Videos_owner_live",
                table: "Videos",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_VideoGroups_owner_live",
                table: "VideoGroups",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_SubjectVideoGroupAssignments_owner_live",
                table: "SubjectVideoGroupAssignments",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_Subjects_owner_live",
                table: "Subjects",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_Projects_owner_live",
                table: "Projects",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_ProjectAccessCodes_owner_live",
                table: "ProjectAccessCodes",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_Labels_owner_live",
                table: "Labels",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_GeneratedReports_owner_live",
                table: "GeneratedReports",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_AssignedLabels_owner_live",
                table: "AssignedLabels",
                column: "CreatedById",
                filter: "\"DelDate\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_Videos_owner_live",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "ix_VideoGroups_owner_live",
                table: "VideoGroups");

            migrationBuilder.DropIndex(
                name: "ix_SubjectVideoGroupAssignments_owner_live",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropIndex(
                name: "ix_Subjects_owner_live",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "ix_Projects_owner_live",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "ix_ProjectAccessCodes_owner_live",
                table: "ProjectAccessCodes");

            migrationBuilder.DropIndex(
                name: "ix_Labels_owner_live",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "ix_GeneratedReports_owner_live",
                table: "GeneratedReports");

            migrationBuilder.DropIndex(
                name: "ix_AssignedLabels_owner_live",
                table: "AssignedLabels");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CreatedById",
                table: "Videos",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VideoGroups_CreatedById",
                table: "VideoGroups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVideoGroupAssignments_CreatedById",
                table: "SubjectVideoGroupAssignments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CreatedById",
                table: "Subjects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccessCodes_CreatedById",
                table: "ProjectAccessCodes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedById",
                table: "Labels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_CreatedById",
                table: "GeneratedReports",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedLabels_CreatedById",
                table: "AssignedLabels",
                column: "CreatedById");
        }
    }
}
