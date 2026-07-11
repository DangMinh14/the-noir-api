using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheNoir.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddToppings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowsToppings",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "Toppings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PriceVnd = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toppings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemToppings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ToppingId = table.Column<int>(type: "INTEGER", nullable: true),
                    ToppingName = table.Column<string>(type: "TEXT", nullable: false),
                    ToppingPriceVnd = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemToppings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemToppings_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemToppings_Toppings_ToppingId",
                        column: x => x.ToppingId,
                        principalTable: "Toppings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemToppings_OrderItemId",
                table: "OrderItemToppings",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemToppings_ToppingId",
                table: "OrderItemToppings",
                column: "ToppingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemToppings");

            migrationBuilder.DropTable(
                name: "Toppings");

            migrationBuilder.DropColumn(
                name: "AllowsToppings",
                table: "Categories");
        }
    }
}
