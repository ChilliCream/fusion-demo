using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Reviews.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGraceHopperUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Mirrors the Accounts service, which adds Grace as user 3 so the
            // Order service's seeded order for userId 3 resolves; she has no
            // reviews yet, exercising the empty reviews connection.
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Grace Hopper" });

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
