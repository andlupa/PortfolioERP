using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderQuantityToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "sales_order_lines",
                type: "integer",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "VatPercentage",
                table: "products",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 22m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatPercentage",
                table: "products");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "sales_order_lines",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}
