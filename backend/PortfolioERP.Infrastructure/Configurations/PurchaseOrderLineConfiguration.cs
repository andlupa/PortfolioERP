using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Persistence.Configurations;

public class PurchaseOrderLineConfiguration
    : IEntityTypeConfiguration<PurchaseOrderLine>
{
    public void Configure(
        EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        builder.ToTable("PurchaseOrderLines");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Quantity)
            .IsRequired();

        builder.Property(l => l.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(l => l.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(l => l.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(l => l.NetAmount)
            .HasPrecision(18, 2);

        builder.Property(l => l.VatPercentage)
            .HasPrecision(5, 2);

        builder.Property(l => l.VatAmount)
            .HasPrecision(18, 2);

        builder.Property(l => l.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasIndex(l => l.PurchaseOrderId);

        builder.HasIndex(l => l.ProductId);

        builder.HasOne(l => l.Product)
            .WithMany(p => p.PurchaseOrderLines)
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}