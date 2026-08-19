using Microsoft.EntityFrameworkCore;

using PortfolioERP.Application.Features.Inventory;
using PortfolioERP.Domain.Entities;
using PortfolioERP.Domain.Enums;
using PortfolioERP.Infrastructure.Persistence;

namespace PortfolioERP.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _context;

    public InventoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task ReceiveAsync(
        int productId,
        int quantity,
        string referenceType,
        int? referenceId,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        Console.WriteLine(
            $"InventoryService.ReceiveAsync: " +
            $"Product={productId}, Quantity={quantity}");

        var inventory =
            await GetInventoryAsync(productId, cancellationToken);

        Console.WriteLine(
            $"Before: OnHand={inventory.QuantityOnHand}");

        // Aumenta la giacenza fisica
        inventory.QuantityOnHand += quantity;

        Console.WriteLine(
            $"After: OnHand={inventory.QuantityOnHand}");

        // Registra il movimento
        AddMovement(
            productId,
            quantity,
            InventoryMovementType.Receipt,
            referenceType,
            referenceId);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReserveAsync(
        int productId,
        int quantity,
        string referenceType,
        int? referenceId,
        CancellationToken cancellationToken = default)
    {

        Console.WriteLine(
            $"Inventory DbContext: {_context.ContextId}");
        
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var inventory =
            await GetInventoryAsync(productId, cancellationToken);

        if (inventory.AvailableQuantity < quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock for product {productId}. " +
                $"Available: {inventory.AvailableQuantity}, " +
                $"requested: {quantity}.");
        }

        inventory.QuantityReserved += quantity;

        AddMovement(
            productId,
            quantity,
            InventoryMovementType.Reservation,
            referenceType,
            referenceId);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReleaseAsync(
        int productId,
        int quantity,
        string referenceType,
        int? referenceId,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var inventory =
            await GetInventoryAsync(productId, cancellationToken);

        if (inventory.QuantityReserved < quantity)
        {
            throw new InvalidOperationException(
                $"Cannot release {quantity} units for product {productId}. " +
                $"Only {inventory.QuantityReserved} units are reserved.");
        }

        inventory.QuantityReserved -= quantity;

        AddMovement(
            productId,
            quantity,
            InventoryMovementType.Release,
            referenceType,
            referenceId);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ShipAsync(
        int productId,
        int quantity,
        string referenceType,
        int? referenceId,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        var inventory =
            await GetInventoryAsync(productId, cancellationToken);

        if (inventory.QuantityReserved < quantity)
        {
            throw new InvalidOperationException(
                $"Product {productId} does not have enough reserved stock.");
        }

        if (inventory.QuantityOnHand < quantity)
        {
            throw new InvalidOperationException(
                $"Product {productId} does not have enough physical stock.");
        }

        inventory.QuantityReserved -= quantity;
        inventory.QuantityOnHand -= quantity;

        AddMovement(
            productId,
            -quantity,
            InventoryMovementType.Shipment,
            referenceType,
            referenceId);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<InventoryItem> GetInventoryAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        var inventory =
            await _context.InventoryItems
                .SingleOrDefaultAsync(
                    x => x.ProductId == productId,
                    cancellationToken);

        if (inventory is null)
        {
            throw new InvalidOperationException(
                $"Inventory for product {productId} does not exist.");
        }

        return inventory;
    }

    private void AddMovement(
        int productId,
        int quantity,
        InventoryMovementType type,
        string referenceType,
        int? referenceId)
    {
        _context.InventoryMovements.Add(
            new InventoryMovement
            {
                ProductId = productId,
                Quantity = quantity,
                Type = type,
                OccurredAtUtc = DateTime.UtcNow,
                ReferenceType = referenceType,
                ReferenceId = referenceId
            });
    }

    public async Task<int> GetAvailableQuantityAsync(
    int productId,
    CancellationToken cancellationToken = default)
    {
        var inventory = await _context.InventoryItems
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.ProductId == productId,
                cancellationToken);

        if (inventory is null)
        {
            throw new InvalidOperationException(
                $"Inventory for product {productId} does not exist.");
        }

        return inventory.QuantityOnHand -
               inventory.QuantityReserved;
    }
}