using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class dodanoowneriddeldate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedLabels_Labelers_LabelerId",
                table: "AssignedLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Scientists_ScientistId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "LabelerProject");

            migrationBuilder.DropTable(
                name: "LabelerSubjectVideoGroupAssignment");

            migrationBuilder.DropTable(
                name: "Scientists");

            migrationBuilder.DropTable(
                name: "Labelers");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ScientistId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_AssignedLabels_LabelerId",
                table: "AssignedLabels");

            migrationBuilder.DropColumn(
                name: "ScientistId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LabelerId",
                table: "AssignedLabels");

            migrationBuilder.AddColumn<DateTime>(
                name: "DelDate",
                table: "Videos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Videos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DelDate",
                table: "VideoGroups",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "VideoGroups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DelDate",
                table: "SubjectVideoGroupAssignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "SubjectVideoGroupAssignments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DelDate",
                table: "Subjects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Subjects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DelDate",
                table: "Projects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DelDate",
                table: "ProjectAccessCodes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "ProjectAccessCodes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DelDate",
                table: "Labels",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Labels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DelDate",
                table: "AssignedLabels",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "AssignedLabels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubjectVideoGroupAssignmentId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_OwnerId",
                table: "Videos",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoGroups_OwnerId",
                table: "VideoGroups",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVideoGroupAssignments_OwnerId",
                table: "SubjectVideoGroupAssignments",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_OwnerId",
                table: "Subjects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccessCodes_OwnerId",
                table: "ProjectAccessCodes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_OwnerId",
                table: "Labels",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedLabels_OwnerId",
                table: "AssignedLabels",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProjectId",
                table: "AspNetUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SubjectVideoGroupAssignmentId",
                table: "AspNetUsers",
                column: "SubjectVideoGroupAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Projects_ProjectId",
                table: "AspNetUsers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SubjectVideoGroupAssignments_SubjectVideoGroupA~",
                table: "AspNetUsers",
                column: "SubjectVideoGroupAssignmentId",
                principalTable: "SubjectVideoGroupAssignments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedLabels_AspNetUsers_OwnerId",
                table: "AssignedLabels",
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
                onDelete: ReferentialAction.Cascade);

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
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Projects_ProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SubjectVideoGroupAssignments_SubjectVideoGroupA~",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedLabels_AspNetUsers_OwnerId",
                table: "AssignedLabels");

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

            migrationBuilder.DropIndex(
                name: "IX_Videos_OwnerId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_VideoGroups_OwnerId",
                table: "VideoGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectVideoGroupAssignments_OwnerId",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_OwnerId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAccessCodes_OwnerId",
                table: "ProjectAccessCodes");

            migrationBuilder.DropIndex(
                name: "IX_Labels_OwnerId",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_AssignedLabels_OwnerId",
                table: "AssignedLabels");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SubjectVideoGroupAssignmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DelDate",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "DelDate",
                table: "VideoGroups");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "VideoGroups");

            migrationBuilder.DropColumn(
                name: "DelDate",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropColumn(
                name: "DelDate",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "DelDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DelDate",
                table: "ProjectAccessCodes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ProjectAccessCodes");

            migrationBuilder.DropColumn(
                name: "DelDate",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "DelDate",
                table: "AssignedLabels");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "AssignedLabels");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubjectVideoGroupAssignmentId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "ScientistId",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LabelerId",
                table: "AssignedLabels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Labelers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labelers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labelers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scientists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scientists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scientists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabelerProject",
                columns: table => new
                {
                    ProjectLabelersId = table.Column<int>(type: "integer", nullable: false),
                    ProjectLabelersId1 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelerProject", x => new { x.ProjectLabelersId, x.ProjectLabelersId1 });
                    table.ForeignKey(
                        name: "FK_LabelerProject_Labelers_ProjectLabelersId",
                        column: x => x.ProjectLabelersId,
                        principalTable: "Labelers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabelerProject_Projects_ProjectLabelersId1",
                        column: x => x.ProjectLabelersId1,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabelerSubjectVideoGroupAssignment",
                columns: table => new
                {
                    LabelersId = table.Column<int>(type: "integer", nullable: false),
                    SubjectVideoGroupsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelerSubjectVideoGroupAssignment", x => new { x.LabelersId, x.SubjectVideoGroupsId });
                    table.ForeignKey(
                        name: "FK_LabelerSubjectVideoGroupAssignment_Labelers_LabelersId",
                        column: x => x.LabelersId,
                        principalTable: "Labelers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabelerSubjectVideoGroupAssignment_SubjectVideoGroupAssignm~",
                        column: x => x.SubjectVideoGroupsId,
                        principalTable: "SubjectVideoGroupAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ScientistId",
                table: "Projects",
                column: "ScientistId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedLabels_LabelerId",
                table: "AssignedLabels",
                column: "LabelerId");

            migrationBuilder.CreateIndex(
                name: "IX_LabelerProject_ProjectLabelersId1",
                table: "LabelerProject",
                column: "ProjectLabelersId1");

            migrationBuilder.CreateIndex(
                name: "IX_Labelers_UserId",
                table: "Labelers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabelerSubjectVideoGroupAssignment_SubjectVideoGroupsId",
                table: "LabelerSubjectVideoGroupAssignment",
                column: "SubjectVideoGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_Scientists_UserId",
                table: "Scientists",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedLabels_Labelers_LabelerId",
                table: "AssignedLabels",
                column: "LabelerId",
                principalTable: "Labelers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Scientists_ScientistId",
                table: "Projects",
                column: "ScientistId",
                principalTable: "Scientists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
