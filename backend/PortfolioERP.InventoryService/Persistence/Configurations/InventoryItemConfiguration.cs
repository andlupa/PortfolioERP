using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioERP.InventoryService.Domain;

namespace PortfolioERP.InventoryService.Persistence.Configurations;

public class InventoryItemConfiguration
    : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(
        EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.QuantityOnHand)
            .IsRequired();

        builder.Property(x => x.QuantityReserved)
            .IsRequired();

        builder.Property(x => x.ReorderLevel)
            .IsRequired();

        builder.Ignore(x => x.AvailableQuantity);

        builder.HasIndex(x => x.ProductId)
            .IsUnique();
    }
}