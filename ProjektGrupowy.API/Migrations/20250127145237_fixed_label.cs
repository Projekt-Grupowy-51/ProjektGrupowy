using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektGrupowy.API.Migrations
{
    /// <inheritdoc />
    public partial class fixed_label : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Vidoes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "VideoGroups");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Scientists");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Scientists");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Labelers");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Labels",
                newName: "Type");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreationDate",
                table: "SubjectVideoGroupAssignments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ModificationDate",
                table: "SubjectVideoGroupAssignments",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreationDate",
                table: "Projects",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ModificationDate",
                table: "Projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "Labels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<char>(
                name: "Shortcut",
                table: "Labels",
                type: "character(1)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "End",
                table: "AssignedLabels",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Start",
                table: "AssignedLabels",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "SubjectVideoGroupAssignments");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ColorHex",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "Shortcut",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "End",
                table: "AssignedLabels");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "AssignedLabels");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Labels",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vidoes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "VideoGroups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Scientists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Scientists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Labelers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
