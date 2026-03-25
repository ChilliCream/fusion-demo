using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Demo.Payments.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "CreatedAt", "OrderId", "Status" },
                values: new object[,]
                {
                    { 1, 200m, new DateTimeOffset(new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc)), 1, 1 },
                    { 2, 17m, new DateTimeOffset(new DateTime(2025, 1, 1, 10, 5, 0, DateTimeKind.Utc)), 2, 2 },
                    { 3, 17m, new DateTimeOffset(new DateTime(2025, 1, 1, 10, 10, 0, DateTimeKind.Utc)), 2, 1 },
                    { 4, 54m, new DateTimeOffset(new DateTime(2025, 1, 1, 10, 15, 0, DateTimeKind.Utc)), 3, 2 }
                });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Payments\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Payments\"), 1), true);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
