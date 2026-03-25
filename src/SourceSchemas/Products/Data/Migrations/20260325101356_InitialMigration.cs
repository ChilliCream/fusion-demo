using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Demo.Products.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    Length = table.Column<double>(type: "double precision", nullable: false),
                    Width = table.Column<double>(type: "double precision", nullable: false),
                    Height = table.Column<double>(type: "double precision", nullable: false),
                    PictureFileName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Height", "Length", "Name", "PictureFileName", "Price", "Weight", "Width" },
                values: new object[,]
                {
                    { 1, 75.0, 200.0, "Table", "table.jpg", 899.99, 100, 100.0 },
                    { 2, 85.0, 220.0, "Couch", "couch.jpg", 1299.5, 1000, 90.0 },
                    { 3, 90.0, 50.0, "Chair", "chair.jpg", 54.0, 50, 50.0 },
                    { 4, 180.0, 120.0, "Bookshelf", "bookshelf.jpg", 349.99, 150, 35.0 },
                    { 5, 75.0, 150.0, "Desk", "desk.jpg", 599.0, 80, 75.0 },
                    { 6, 40.0, 210.0, "Bed Frame", "bed-frame.jpg", 799.99, 200, 160.0 },
                    { 7, 60.0, 50.0, "Nightstand", "nightstand.jpg", 149.5, 30, 40.0 },
                    { 8, 45.0, 120.0, "Coffee Table", "coffee-table.jpg", 299.99, 45, 60.0 },
                    { 9, 95.0, 45.0, "Dining Chair", "dining-chair.jpg", 89.99, 12, 50.0 },
                    { 10, 220.0, 200.0, "Wardrobe", "wardrobe.jpg", 1499.0, 250, 60.0 },
                    { 11, 55.0, 180.0, "TV Stand", "tv-stand.jpg", 449.0, 65, 45.0 },
                    { 12, 90.0, 150.0, "Dresser", "dresser.jpg", 699.99, 120, 50.0 },
                    { 13, 95.0, 85.0, "Armchair", "armchair.jpg", 549.5, 35, 80.0 },
                    { 14, 110.0, 40.0, "Bar Stool", "bar-stool.jpg", 129.99, 15, 40.0 },
                    { 15, 85.0, 180.0, "Sideboard", "sideboard.jpg", 899.0, 140, 45.0 },
                    { 16, 45.0, 120.0, "Bench", "bench.jpg", 179.5, 22, 40.0 },
                    { 17, 110.0, 70.0, "Rocking Chair", "rocking-chair.jpg", 399.99, 28, 85.0 },
                    { 18, 150.0, 100.0, "Storage Cabinet", "storage-cabinet.jpg", 499.0, 95, 45.0 }
                });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Products\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Products\"), 1), true);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
