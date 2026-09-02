using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Accounts.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewerUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Birthdate", "Name", "Username" },
                values: new object[,]
                {
                    { 4, new DateTime(1936, 8, 17, 0, 0, 0, DateTimeKind.Utc), "Margaret Hamilton", "@margaret" },
                    { 5, new DateTime(1941, 9, 9, 0, 0, 0, DateTimeKind.Utc), "Dennis Ritchie", "@dennis" },
                    { 6, new DateTime(1939, 11, 7, 0, 0, 0, DateTimeKind.Utc), "Barbara Liskov", "@barbara" },
                    { 7, new DateTime(1969, 12, 28, 0, 0, 0, DateTimeKind.Utc), "Linus Torvalds", "@linus" },
                    { 8, new DateTime(1938, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Donald Knuth", "@donald" },
                    { 9, new DateTime(1930, 5, 11, 0, 0, 0, DateTimeKind.Utc), "Edsger Dijkstra", "@edsger" },
                    { 10, new DateTime(1918, 8, 26, 0, 0, 0, DateTimeKind.Utc), "Katherine Johnson", "@katherine" },
                    { 11, new DateTime(1955, 6, 8, 0, 0, 0, DateTimeKind.Utc), "Tim Berners-Lee", "@tim" },
                    { 12, new DateTime(1932, 8, 4, 0, 0, 0, DateTimeKind.Utc), "Frances Allen", "@frances" },
                    { 13, new DateTime(1943, 2, 4, 0, 0, 0, DateTimeKind.Utc), "Ken Thompson", "@ken" }
                });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), true);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValues: new object[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), true);");
        }
    }
}
