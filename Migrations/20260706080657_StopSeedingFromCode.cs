using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheNoir.Api.Migrations
{
    /// <inheritdoc />
    public partial class StopSeedingFromCode : Migration
    {
        // Seed data was removed from the model: rows in Products and Cities are
        // now owned by the database, not by code. On a database that already
        // has these rows (from InitialCreate's HasData), leave them alone.
        // On a fresh replay (new dev machine, or this DB was reset), delete
        // the rows InitialCreate just inserted so later migrations that add
        // required columns (e.g. Products.CategoryId) don't have to invent
        // values for placeholder test data.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Products;");
            migrationBuilder.Sql("DELETE FROM Cities;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
