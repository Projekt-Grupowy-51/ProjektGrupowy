using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class changed_fields_in_report : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "GeneratedReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GeneratedReports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
