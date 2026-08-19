using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInventoryFromProductStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO InventoryItems
                    (ProductId, QuantityOnHand, QuantityReserved, ReorderLevel)
                SELECT
                    Id,
                    StockQuantity,
                    0,
                    0
                FROM Products
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM InventoryItems i
                    WHERE i.ProductId = Products.Id
                );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
