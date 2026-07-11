using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheNoir.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderExpiryAndAutoCancel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoCancelled",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Existing rows get a real deadline (CreatedAt + 24h) instead of
            // the default-value epoch, so the auto-cancel job doesn't treat
            // every pre-existing open order as already expired.
            migrationBuilder.Sql("UPDATE Orders SET ExpiresAt = datetime(CreatedAt, '+24 hours');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoCancelled",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Orders");
        }
    }
}
