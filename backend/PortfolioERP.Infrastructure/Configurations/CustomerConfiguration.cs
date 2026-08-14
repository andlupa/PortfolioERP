using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioERP.Domain.Entities;

namespace PortfolioERP.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		builder.ToTable("Customers");

		builder.HasKey(c => c.Id);

		builder.Property(c => c.CustomerCode)
			.HasMaxLength(30)
			.IsRequired();

		builder.Property(c => c.CompanyName)
			.HasMaxLength(200)
			.IsRequired();

		builder.Property(c => c.Email)
			.HasMaxLength(200)
			.IsRequired();

		builder.Property(c => c.Phone)
			.HasMaxLength(30);

		builder.Property(c => c.Address)
			.HasMaxLength(250);

		builder.Property(c => c.City)
			.HasMaxLength(100);

		builder.Property(c => c.Province)
			.HasMaxLength(10);

		builder.Property(c => c.PostalCode)
			.HasMaxLength(15);

		builder.Property(c => c.Country)
			.HasMaxLength(100);

		builder.HasIndex(c => c.CustomerCode)
			.IsUnique();

		builder.HasIndex(c => c.Email);

		builder.HasIndex(c => c.VatNumber)
			.IsUnique();

		builder.HasIndex(c => c.TaxCode)
			.IsUnique();
	}
}