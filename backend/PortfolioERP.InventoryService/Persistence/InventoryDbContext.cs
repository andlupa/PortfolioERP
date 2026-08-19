using Microsoft.EntityFrameworkCore;
using PortfolioERP.InventoryService.Domain;
using PortfolioERP.InventoryService.Entities;

namespace PortfolioERP.InventoryService.Persistence;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(
        DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems =>
        Set<InventoryItem>();

    public DbSet<InventoryMovement> InventoryMovements =>
        Set<InventoryMovement>();

    public DbSet<ProcessedMessage> ProcessedMessages =>
        Set<ProcessedMessage>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProcessedMessage>()
            .HasIndex(x => new
            {
                x.MessageType,
                x.MessageId
            })
            .IsUnique();

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(InventoryDbContext).Assembly);
    }

}