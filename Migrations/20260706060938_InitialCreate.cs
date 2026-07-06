using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TheNoir.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MaisonCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Kind = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PriceVnd = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ImageAlt = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Address", "Kind", "MaisonCount", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "42 Vườn Trà", "Flagship", 9, "Sài Gòn", 1 },
                    { 2, "18 Sương Mai", "Salon", 5, "Hà Nội", 2 },
                    { 3, "27 Bến Lá", "Riverside", 3, "Đà Nẵng", 3 },
                    { 4, "15 Đồi Mây", "Garden house", 1, "Huế", 4 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "ImageAlt", "ImageUrl", "Name", "PriceVnd", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Milk Tea", "Our house black tea folded into silky fresh milk, finished with burnt-caramel pearls.", "A glass of milk tea with tapioca pearls on a dark table", "https://images.unsplash.com/photo-1558857563-b371033873b8?q=80&w=1200&auto=format&fit=crop", "Noir Signature", 65000, 1 },
                    { 2, "Pure Tea", "Hand-rolled oolong from estates at 1,200 m: orchid nose, honeyed finish, and steep after steep.", "Hot tea being poured from a teapot into a small cup", "https://images.unsplash.com/photo-1564890369478-c89ca6d9cde9?q=80&w=1200&auto=format&fit=crop", "Highland Oolong", 58000, 2 },
                    { 3, "Coffee", "Đà Lạt arabica dripped through a traditional phin, layered over condensed milk.", "A dark cup of Vietnamese coffee in moody light", "https://images.unsplash.com/photo-1509042239860-f550ce710b93?q=80&w=1200&auto=format&fit=crop", "Phin Noir", 55000, 3 },
                    { 4, "Matcha", "Stone-ground ceremonial matcha whisked over cold jasmine milk, grassy and bright.", "An iced green matcha drink in a tall glass", "https://images.unsplash.com/photo-1556679343-c7306c1976bc?q=80&w=1200&auto=format&fit=crop", "Jade Cloud", 72000, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
