using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class addedscientist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wideo_Projekty_ProjectId",
                table: "Wideo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wideo",
                table: "Wideo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projekty",
                table: "Projekty");

            migrationBuilder.RenameTable(
                name: "Wideo",
                newName: "Vidoes");

            migrationBuilder.RenameTable(
                name: "Projekty",
                newName: "Projects");

            migrationBuilder.RenameIndex(
                name: "IX_Wideo_ProjectId",
                table: "Vidoes",
                newName: "IX_Vidoes_ProjectId");

            migrationBuilder.AddColumn<int>(
                name: "ScientistId",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vidoes",
                table: "Vidoes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projects",
                table: "Projects",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Scientists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scientists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ScientistId",
                table: "Projects",
                column: "ScientistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Scientists_ScientistId",
                table: "Projects",
                column: "ScientistId",
                principalTable: "Scientists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vidoes_Projects_ProjectId",
                table: "Vidoes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Scientists_ScientistId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Vidoes_Projects_ProjectId",
                table: "Vidoes");

            migrationBuilder.DropTable(
                name: "Scientists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vidoes",
                table: "Vidoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Projects",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ScientistId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ScientistId",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "Vidoes",
                newName: "Wideo");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Projekty");

            migrationBuilder.RenameIndex(
                name: "IX_Vidoes_ProjectId",
                table: "Wideo",
                newName: "IX_Wideo_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wideo",
                table: "Wideo",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Projekty",
                table: "Projekty",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wideo_Projekty_ProjectId",
                table: "Wideo",
                column: "ProjectId",
                principalTable: "Projekty",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
