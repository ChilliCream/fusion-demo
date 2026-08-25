using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Accounts.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGraceHopperUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The Order service seeds an order for userId 3, which did not
            // exist here; Grace joins Ada and Alan so that order resolves.
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Birthdate", "Name", "Username" },
                values: new object[] { 3, new DateTime(1906, 12, 9, 0, 0, 0, DateTimeKind.Utc), "Grace Hopper", "@grace" });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), true);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), true);");
        }
    }
}
