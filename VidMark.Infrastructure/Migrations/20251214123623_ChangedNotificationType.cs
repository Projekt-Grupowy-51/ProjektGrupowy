using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VidMark.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNotificationType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "message",
                table: "domain_events");

            migrationBuilder.AddColumn<string>(
                name: "message_content",
                table: "domain_events",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "message_content",
                table: "domain_events");

            migrationBuilder.AddColumn<string>(
                name: "message",
                table: "domain_events",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
