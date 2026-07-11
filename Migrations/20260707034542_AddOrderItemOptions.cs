using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheNoir.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IceOption",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SugarLevel",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Temperature",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IceOption",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SugarLevel",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "OrderItems");
        }
    }
}
