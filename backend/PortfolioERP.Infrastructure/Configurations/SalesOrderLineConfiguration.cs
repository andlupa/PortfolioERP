using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Persistence.Configurations;

public class SalesOrderLineConfiguration
    : IEntityTypeConfiguration<SalesOrderLine>
{
    public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        builder.ToTable("SalesOrderLines");

        builder.HasKey(line => line.Id);

        builder.Property(line => line.Quantity)
            .IsRequired();

        builder.Property(line => line.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(line => line.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(line => line.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(line => line.VatPercentage)
            .HasPrecision(5, 2);

        builder.Property(line => line.VatAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 2);

        builder.Property(line => line.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(line => line.Product)
            .WithMany(product => product.OrderLines)
            .HasForeignKey(line => line.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}