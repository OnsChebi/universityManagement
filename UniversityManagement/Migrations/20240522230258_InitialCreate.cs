using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityManagement.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Persons",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Persons",
                newName: "BirthDate");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Categories",
                newName: "PostOrClass");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Persons",
                type: "nvarchar(75)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Persons",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "Persons",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "PostOrClass",
                table: "Categories",
                newName: "Title");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Categories",
                type: "nvarchar(5)",
                nullable: false,
                defaultValue: "");
        }
    }
}
