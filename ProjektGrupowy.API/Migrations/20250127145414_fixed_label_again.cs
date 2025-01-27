using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class fixed_label_again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ColorHex",
                table: "Labels",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ColorHex",
                table: "Labels",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(7)",
                oldMaxLength: 7);
        }
    }
}
