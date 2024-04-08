using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksReadTrackerDBLibrary.Migrations
{
    /// <inheritdoc />
    public partial class PagesRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PagesRead",
                table: "Books",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TotalPages",
                table: "Books",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PagesRead",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TotalPages",
                table: "Books");
        }
    }
}
