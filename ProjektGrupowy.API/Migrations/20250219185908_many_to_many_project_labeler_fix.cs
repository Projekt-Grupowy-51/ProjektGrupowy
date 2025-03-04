using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class many_to_many_project_labeler_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_LabelerProject_ProjectLabelersId1",
                table: "LabelerProject",
                column: "ProjectLabelersId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabelerProject");
        }
    }
}
