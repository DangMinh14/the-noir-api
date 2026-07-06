using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheNoir.Api.Migrations
{
    /// <inheritdoc />
    public partial class StopSeedingFromCode : Migration
    {
        // Seed data was removed from the model: rows in Products and Cities are
        // now owned by the database, not by code. The scaffolded DeleteData /
        // InsertData calls were stripped so existing rows are left untouched.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
