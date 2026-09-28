using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Cart.Data.Migrations
{
    /// <inheritdoc />
    public partial class ScopeCartToOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Carts are now scoped to the signed-in shopper's "sub" claim. Pre-existing
            // rows have no owner to assign them to, so this demo data is dropped rather
            // than backfilled.
            migrationBuilder.Sql("DELETE FROM \"CartItems\";");
            migrationBuilder.Sql("DELETE FROM \"Carts\";");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Carts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_OwnerId",
                table: "Carts",
                column: "OwnerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_OwnerId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Carts");
        }
    }
}
