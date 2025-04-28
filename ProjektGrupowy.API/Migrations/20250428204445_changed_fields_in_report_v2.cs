using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class changed_fields_in_report_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GeneratedReports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "GeneratedReports");
        }
    }
}
