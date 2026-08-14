using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Persistence.Configurations;

public class SalesOrderConfiguration
    : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.OrderNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(order => order.OrderNumber)
            .IsUnique();

        builder.Property(order => order.Status)
            .HasConversion<int>();

        builder.Property(order => order.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(order => order.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(order => order.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(order => order.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(order => order.Customer)
            .WithMany(customer => customer.Orders)
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Lines)
            .WithOne(line => line.SalesOrder)
            .HasForeignKey(line => line.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}