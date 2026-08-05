using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.ToTable("products");

		builder.HasKey(p => p.Id);

		builder.Property(p => p.Code)
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(p => p.Name)
			.HasMaxLength(150)
			.IsRequired();

		builder.Property(p => p.Description)
			.HasMaxLength(1000);

		builder.Property(p => p.Price)
			.HasPrecision(18, 2);
	
		builder.Property(p => p.VatPercentage)
			.HasPrecision(5, 2)
			.HasDefaultValue(22m);

		builder.HasIndex(p => p.Code)
			.IsUnique();

		builder.HasIndex(p => p.Name);

		builder.HasOne(p => p.Category)
			.WithMany(c => c.Products)
			.HasForeignKey(p => p.CategoryId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}