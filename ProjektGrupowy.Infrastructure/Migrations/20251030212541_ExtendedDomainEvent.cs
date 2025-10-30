using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedDomainEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventData",
                table: "DomainEvents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "DomainEvents",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventData",
                table: "DomainEvents");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "DomainEvents");
        }
    }
}
