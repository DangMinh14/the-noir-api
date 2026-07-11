using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheNoir.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDescriptionHtml : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionHtml",
                table: "Products",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionHtml",
                table: "Products");
        }
    }
}
