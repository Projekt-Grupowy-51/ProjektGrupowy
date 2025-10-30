using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjektGrupowy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DomainEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ModificationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedReports_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneratedReports_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAccessCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAccessCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAccessCodes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectAccessCodes_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLabelers",
                columns: table => new
                {
                    LabeledProjectsId = table.Column<int>(type: "integer", nullable: false),
                    ProjectLabelersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLabelers", x => new { x.LabeledProjectsId, x.ProjectLabelersId });
                    table.ForeignKey(
                        name: "FK_ProjectLabelers_Projects_LabeledProjectsId",
                        column: x => x.LabeledProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectLabelers_user_entity_ProjectLabelersId",
                        column: x => x.ProjectLabelersId,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoGroups_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoGroups_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ColorHex = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Shortcut = table.Column<char>(type: "character(1)", nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labels_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Labels_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectVideoGroupAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ModificationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    VideoGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectVideoGroupAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectVideoGroupAssignments_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectVideoGroupAssignments_VideoGroups_VideoGroupId",
                        column: x => x.VideoGroupId,
                        principalTable: "VideoGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectVideoGroupAssignments_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PositionInQueue = table.Column<int>(type: "integer", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    VideoGroupId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_VideoGroups_VideoGroupId",
                        column: x => x.VideoGroupId,
                        principalTable: "VideoGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Videos_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabelersAssignments",
                columns: table => new
                {
                    LabeledAssignmentsId = table.Column<int>(type: "integer", nullable: false),
                    LabelersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelersAssignments", x => new { x.LabeledAssignmentsId, x.LabelersId });
                    table.ForeignKey(
                        name: "FK_LabelersAssignments_SubjectVideoGroupAssignments_LabeledAss~",
                        column: x => x.LabeledAssignmentsId,
                        principalTable: "SubjectVideoGroupAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabelersAssignments_user_entity_LabelersId",
                        column: x => x.LabelersId,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignedLabels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LabelId = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    VideoId = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<string>(type: "text", nullable: false),
                    End = table.Column<string>(type: "text", nullable: false),
                    InsDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignedLabels_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignedLabels_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignedLabels_user_entity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedLabels_CreatedById",
                table: "AssignedLabels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedLabels_LabelId",
                table: "AssignedLabels",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedLabels_VideoId",
                table: "AssignedLabels",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvents_IsPublished",
                table: "DomainEvents",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_CreatedById",
                table: "GeneratedReports",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_Path",
                table: "GeneratedReports",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_ProjectId",
                table: "GeneratedReports",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LabelersAssignments_LabelersId",
                table: "LabelersAssignments",
                column: "LabelersId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CreatedById",
                table: "Labels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_SubjectId",
                table: "Labels",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccessCode_Code",
                table: "ProjectAccessCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccessCodes_CreatedById",
                table: "ProjectAccessCodes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccessCodes_ProjectId",
                table: "ProjectAccessCodes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLabelers_ProjectLabelersId",
                table: "ProjectLabelers",
                column: "ProjectLabelersId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CreatedById",
                table: "Subjects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ProjectId",
                table: "Subjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVideoGroupAssignments_CreatedById",
                table: "SubjectVideoGroupAssignments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVideoGroupAssignments_SubjectId",
                table: "SubjectVideoGroupAssignments",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectVideoGroupAssignments_VideoGroupId",
                table: "SubjectVideoGroupAssignments",
                column: "VideoGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoGroups_CreatedById",
                table: "VideoGroups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VideoGroups_ProjectId",
                table: "VideoGroups",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CreatedById",
                table: "Videos",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoGroupId_PositionInQueue",
                table: "Videos",
                columns: new[] { "VideoGroupId", "PositionInQueue" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedLabels");

            migrationBuilder.DropTable(
                name: "DomainEvents");

            migrationBuilder.DropTable(
                name: "GeneratedReports");

            migrationBuilder.DropTable(
                name: "LabelersAssignments");

            migrationBuilder.DropTable(
                name: "ProjectAccessCodes");

            migrationBuilder.DropTable(
                name: "ProjectLabelers");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "SubjectVideoGroupAssignments");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "VideoGroups");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
