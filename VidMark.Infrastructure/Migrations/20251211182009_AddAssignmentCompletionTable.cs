using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VidMark.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentCompletionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assignment_completions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subject_video_group_assignment_id = table.Column<int>(type: "integer", nullable: false),
                    labeler_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assignment_completions", x => x.id);
                    table.ForeignKey(
                        name: "fk_assignment_completions_subject_video_group_assignments_subj",
                        column: x => x.subject_video_group_assignment_id,
                        principalTable: "subject_video_group_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_assignment_completions_users_labeler_id",
                        column: x => x.labeler_id,
                        principalTable: "user_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assignment_completions_labeler_id",
                table: "assignment_completions",
                column: "labeler_id");

            migrationBuilder.CreateIndex(
                name: "ix_assignment_completions_subject_video_group_assignment_id_la",
                table: "assignment_completions",
                columns: new[] { "subject_video_group_assignment_id", "labeler_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assignment_completions");
        }
    }
}
