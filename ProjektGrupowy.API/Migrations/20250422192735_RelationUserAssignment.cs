using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class RelationUserAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SubjectVideoGroupAssignments_SubjectVideoGroupA~",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectVideoGroupAssignments_AspNetUsers_OwnerId",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SubjectVideoGroupAssignmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubjectVideoGroupAssignmentId",
                table: "AspNetUsers");

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
                        name: "FK_LabelersAssignments_AspNetUsers_LabelersId",
                        column: x => x.LabelersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabelersAssignments_SubjectVideoGroupAssignments_LabeledAss~",
                        column: x => x.LabeledAssignmentsId,
                        principalTable: "SubjectVideoGroupAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabelersAssignments_LabelersId",
                table: "LabelersAssignments",
                column: "LabelersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectVideoGroupAssignments_AspNetUsers_OwnerId",
                table: "SubjectVideoGroupAssignments",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectVideoGroupAssignments_AspNetUsers_OwnerId",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropTable(
                name: "LabelersAssignments");

            migrationBuilder.AddColumn<int>(
                name: "SubjectVideoGroupAssignmentId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SubjectVideoGroupAssignmentId",
                table: "AspNetUsers",
                column: "SubjectVideoGroupAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SubjectVideoGroupAssignments_SubjectVideoGroupA~",
                table: "AspNetUsers",
                column: "SubjectVideoGroupAssignmentId",
                principalTable: "SubjectVideoGroupAssignments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_OwnerId",
                table: "Projects",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectVideoGroupAssignments_AspNetUsers_OwnerId",
                table: "SubjectVideoGroupAssignments",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
