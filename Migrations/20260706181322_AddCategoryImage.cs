using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheNoir.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageAlt",
                table: "Categories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageAlt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Categories");
        }
    }
}
