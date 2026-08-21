using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PortfolioERP.InvoiceService.Domain;

namespace PortfolioERP.InvoiceService.Persistence.Configurations;

public sealed class InvoiceLineConfiguration
    : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(
        EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountPercentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.VatPercentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.VatAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);
    }
}