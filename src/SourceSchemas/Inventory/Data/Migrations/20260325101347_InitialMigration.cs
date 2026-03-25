using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Demo.Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "Id", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 10 },
                    { 2, 2, 5 },
                    { 3, 3, 20 },
                    { 4, 4, 15 },
                    { 5, 5, 8 },
                    { 6, 6, 12 },
                    { 7, 7, 25 },
                    { 8, 8, 18 },
                    { 9, 9, 30 },
                    { 10, 10, 0 },
                    { 11, 11, 14 },
                    { 12, 12, 9 },
                    { 13, 13, 22 },
                    { 14, 14, 35 },
                    { 15, 15, 0 },
                    { 16, 16, 28 },
                    { 17, 17, 11 },
                    { 18, 18, 16 }
                });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Inventory\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Inventory\"), 1), true);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory");
        }
    }
}
