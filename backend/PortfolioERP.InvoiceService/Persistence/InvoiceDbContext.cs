using Microsoft.EntityFrameworkCore;
using PortfolioERP.InvoiceService.Domain;

namespace PortfolioERP.InvoiceService.Persistence;

public class InvoiceDbContext : DbContext
{
    public InvoiceDbContext(
        DbContextOptions<InvoiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceDbContext).Assembly);
    }
}