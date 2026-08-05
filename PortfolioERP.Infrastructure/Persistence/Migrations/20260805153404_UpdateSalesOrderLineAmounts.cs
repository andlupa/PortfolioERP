using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSalesOrderLineAmounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LineTotal",
                table: "sales_order_lines",
                newName: "VatAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "NetAmount",
                table: "sales_order_lines",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "sales_order_lines",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetAmount",
                table: "sales_order_lines");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "sales_order_lines");

            migrationBuilder.RenameColumn(
                name: "VatAmount",
                table: "sales_order_lines",
                newName: "LineTotal");
        }
    }
}
