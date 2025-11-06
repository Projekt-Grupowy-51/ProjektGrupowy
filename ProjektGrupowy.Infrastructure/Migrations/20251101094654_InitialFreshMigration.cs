using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjektGrupowy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialFreshMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "domain_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    user_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    event_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    event_data = table.Column<string>(type: "text", nullable: true),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_domain_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    creation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    modification_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "generated_reports",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_generated_reports", x => x.id);
                    table.ForeignKey(
                        name: "fk_generated_reports_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_generated_reports_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_access_codes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_access_codes", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_access_codes_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_access_codes_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_labelers",
                columns: table => new
                {
                    labeled_projects_id = table.Column<int>(type: "integer", nullable: false),
                    project_labelers_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_labelers", x => new { x.labeled_projects_id, x.project_labelers_id });
                    table.ForeignKey(
                        name: "fk_project_labelers_projects_labeled_projects_id",
                        column: x => x.labeled_projects_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_labelers_user_entity_project_labelers_id",
                        column: x => x.project_labelers_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subjects", x => x.id);
                    table.ForeignKey(
                        name: "fk_subjects_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_subjects_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "video_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_video_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_video_groups_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_video_groups_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "labels",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    color_hex = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    shortcut = table.Column<char>(type: "character(1)", nullable: true),
                    subject_id = table.Column<int>(type: "integer", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labels", x => x.id);
                    table.ForeignKey(
                        name: "fk_labels_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_labels_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subject_video_group_assignments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    creation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    modification_date = table.Column<DateOnly>(type: "date", nullable: true),
                    subject_id = table.Column<int>(type: "integer", nullable: false),
                    video_group_id = table.Column<int>(type: "integer", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subject_video_group_assignments", x => x.id);
                    table.ForeignKey(
                        name: "fk_subject_video_group_assignments_subjects_subject_id",
                        column: x => x.subject_id,
                        principalTable: "subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_subject_video_group_assignments_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_subject_video_group_assignments_video_groups_video_group_id",
                        column: x => x.video_group_id,
                        principalTable: "video_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "videos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    position_in_queue = table.Column<int>(type: "integer", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    video_group_id = table.Column<int>(type: "integer", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_videos", x => x.id);
                    table.ForeignKey(
                        name: "fk_videos_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_videos_video_groups_video_group_id",
                        column: x => x.video_group_id,
                        principalTable: "video_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "labelers_assignments",
                columns: table => new
                {
                    labeled_assignments_id = table.Column<int>(type: "integer", nullable: false),
                    labelers_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labelers_assignments", x => new { x.labeled_assignments_id, x.labelers_id });
                    table.ForeignKey(
                        name: "fk_labelers_assignments_subject_video_group_assignments_labele",
                        column: x => x.labeled_assignments_id,
                        principalTable: "subject_video_group_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_labelers_assignments_user_entity_labelers_id",
                        column: x => x.labelers_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assigned_labels",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    label_id = table.Column<int>(type: "integer", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    video_id = table.Column<int>(type: "integer", nullable: false),
                    start = table.Column<string>(type: "text", nullable: false),
                    end = table.Column<string>(type: "text", nullable: false),
                    ins_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    del_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assigned_labels", x => x.id);
                    table.ForeignKey(
                        name: "fk_assigned_labels_labels_label_id",
                        column: x => x.label_id,
                        principalTable: "labels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_assigned_labels_user_entity_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_assigned_labels_videos_video_id",
                        column: x => x.video_id,
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assigned_labels_created_by_id",
                table: "assigned_labels",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_assigned_labels_label_id",
                table: "assigned_labels",
                column: "label_id");

            migrationBuilder.CreateIndex(
                name: "ix_assigned_labels_video_id",
                table: "assigned_labels",
                column: "video_id");

            migrationBuilder.CreateIndex(
                name: "ix_domain_events_is_published",
                table: "domain_events",
                column: "is_published");

            migrationBuilder.CreateIndex(
                name: "ix_generated_reports_created_by_id",
                table: "generated_reports",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_generated_reports_path",
                table: "generated_reports",
                column: "path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_generated_reports_project_id",
                table: "generated_reports",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_labelers_assignments_labelers_id",
                table: "labelers_assignments",
                column: "labelers_id");

            migrationBuilder.CreateIndex(
                name: "ix_labels_created_by_id",
                table: "labels",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_labels_subject_id",
                table: "labels",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_access_codes_created_by_id",
                table: "project_access_codes",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_access_codes_project_id",
                table: "project_access_codes",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccessCode_Code",
                table: "project_access_codes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_project_labelers_project_labelers_id",
                table: "project_labelers",
                column: "project_labelers_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_created_by_id",
                table: "projects",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_subject_video_group_assignments_created_by_id",
                table: "subject_video_group_assignments",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_subject_video_group_assignments_subject_id",
                table: "subject_video_group_assignments",
                column: "subject_id");

            migrationBuilder.CreateIndex(
                name: "ix_subject_video_group_assignments_video_group_id",
                table: "subject_video_group_assignments",
                column: "video_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_subjects_created_by_id",
                table: "subjects",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_subjects_project_id",
                table: "subjects",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_video_groups_created_by_id",
                table: "video_groups",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_video_groups_project_id",
                table: "video_groups",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_created_by_id",
                table: "videos",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_videos_video_group_id_position_in_queue",
                table: "videos",
                columns: new[] { "video_group_id", "position_in_queue" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assigned_labels");

            migrationBuilder.DropTable(
                name: "domain_events");

            migrationBuilder.DropTable(
                name: "generated_reports");

            migrationBuilder.DropTable(
                name: "labelers_assignments");

            migrationBuilder.DropTable(
                name: "project_access_codes");

            migrationBuilder.DropTable(
                name: "project_labelers");

            migrationBuilder.DropTable(
                name: "labels");

            migrationBuilder.DropTable(
                name: "videos");

            migrationBuilder.DropTable(
                name: "subject_video_group_assignments");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropTable(
                name: "video_groups");

            migrationBuilder.DropTable(
                name: "projects");
        }
    }
}
