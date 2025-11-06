using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedVideoQuantities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "available_qualities",
                table: "videos",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "available_qualities",
                table: "videos");
        }
    }
}
