using Microsoft.EntityFrameworkCore;
using PortfolioERP.InventoryService.Clients;
using PortfolioERP.InventoryService.Domain;
using PortfolioERP.InventoryService.DTOs;
using PortfolioERP.InventoryService.Persistence;

namespace PortfolioERP.InventoryService.Services;

public class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _context;
    private readonly IProductClient _productClient;

    public InventoryService(
        InventoryDbContext context,
        IProductClient productClient)
    {
        _context = context;
        _productClient = productClient;
    }

    public async Task<InventoryResponse?> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .Select(x => new InventoryResponse(
                x.ProductId,
                x.QuantityOnHand,
                x.QuantityReserved,
                x.QuantityOnHand - x.QuantityReserved,
                x.ReorderLevel))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InventoryResponse> CreateAsync(
        CreateInventoryRequest request,
        CancellationToken cancellationToken)
    {
        var exists = await _productClient.ProductExistsAsync(
            request.ProductId,
            cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                $"Product {request.ProductId} does not exist.");
        }

        var inventoryExists =
            await _context.InventoryItems.AnyAsync(
                x => x.ProductId == request.ProductId,
                cancellationToken);

        if (inventoryExists)
        {
            throw new InvalidOperationException(
                $"Inventory for product {request.ProductId} already exists.");
        }

        var item = new InventoryItem
        {
            ProductId = request.ProductId,
            QuantityOnHand = request.InitialQuantity,
            QuantityReserved = 0,
            ReorderLevel = request.ReorderLevel
        };

        _context.InventoryItems.Add(item);

        if (request.InitialQuantity != 0)
        {
            _context.InventoryMovements.Add(
                new InventoryMovement
                {
                    ProductId = request.ProductId,
                    Quantity = request.InitialQuantity,
                    Type = InventoryMovementType.Adjustment,
                    OccurredAtUtc = DateTime.UtcNow,
                    ReferenceType = "InitialInventory"
                });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new InventoryResponse(
            item.ProductId,
            item.QuantityOnHand,
            item.QuantityReserved,
            item.AvailableQuantity,
            item.ReorderLevel);
    }

    public async Task<InventoryResponse> ReceiveAsync(
    ReceiveInventoryRequest request,
    CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(
                x => x.ProductId == request.ProductId,
                cancellationToken);

        if (item is null)
        {
            throw new InvalidOperationException(
                $"Inventory for product {request.ProductId} does not exist.");
        }

        item.QuantityOnHand += request.Quantity;

        var movement = new InventoryMovement
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Type = InventoryMovementType.Receipt,
            OccurredAtUtc = DateTime.UtcNow,
            ReferenceType = request.ReferenceType,
            ReferenceId = request.ReferenceId
        };

        _context.InventoryMovements.Add(movement);

        await _context.SaveChangesAsync(cancellationToken);

        return new InventoryResponse(
            item.ProductId,
            item.QuantityOnHand,
            item.QuantityReserved,
            item.AvailableQuantity,
            item.ReorderLevel);
    }

    public async Task<InventoryResponse> ReserveAsync(
    ReserveInventoryRequest request,
    CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(
                x => x.ProductId == request.ProductId,
                cancellationToken);

        if (item is null)
        {
            throw new InvalidOperationException(
                $"Inventory for product {request.ProductId} does not exist.");
        }

        if (item.AvailableQuantity < request.Quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock for product {request.ProductId}. " +
                $"Available: {item.AvailableQuantity}, " +
                $"requested: {request.Quantity}.");
        }

        item.QuantityReserved += request.Quantity;

        var movement = new InventoryMovement
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Type = InventoryMovementType.Reservation,
            OccurredAtUtc = DateTime.UtcNow,
            ReferenceType = request.ReferenceType,
            ReferenceId = request.ReferenceId
        };

        _context.InventoryMovements.Add(movement);

        await _context.SaveChangesAsync(cancellationToken);

        return new InventoryResponse(
            item.ProductId,
            item.QuantityOnHand,
            item.QuantityReserved,
            item.AvailableQuantity,
            item.ReorderLevel);
    }

    public async Task<InventoryResponse> ReleaseAsync(
    ReleaseInventoryRequest request,
    CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(
                x => x.ProductId == request.ProductId,
                cancellationToken);

        if (item is null)
        {
            throw new InvalidOperationException(
                $"Inventory for product {request.ProductId} does not exist.");
        }

        if (item.QuantityReserved < request.Quantity)
        {
            throw new InvalidOperationException(
                $"Cannot release {request.Quantity} units. " +
                $"Only {item.QuantityReserved} units are reserved.");
        }

        item.QuantityReserved -= request.Quantity;

        _context.InventoryMovements.Add(
            new InventoryMovement
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Type = InventoryMovementType.Release,
                OccurredAtUtc = DateTime.UtcNow,
                ReferenceType = request.ReferenceType,
                ReferenceId = request.ReferenceId
            });

        await _context.SaveChangesAsync(cancellationToken);

        return new InventoryResponse(
            item.ProductId,
            item.QuantityOnHand,
            item.QuantityReserved,
            item.AvailableQuantity,
            item.ReorderLevel);
    }

    public async Task<InventoryResponse> ShipAsync(
    ShipInventoryRequest request,
    CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(
                x => x.ProductId == request.ProductId,
                cancellationToken);

        if (item is null)
        {
            throw new InvalidOperationException(
                $"Inventory for product {request.ProductId} does not exist.");
        }

        if (item.QuantityReserved < request.Quantity)
        {
            throw new InvalidOperationException(
                $"Cannot ship {request.Quantity} units. " +
                $"Only {item.QuantityReserved} units are reserved.");
        }

        item.QuantityOnHand -= request.Quantity;
        item.QuantityReserved -= request.Quantity;

        _context.InventoryMovements.Add(
            new InventoryMovement
            {
                ProductId = request.ProductId,
                Quantity = -request.Quantity,
                Type = InventoryMovementType.Shipment,
                OccurredAtUtc = DateTime.UtcNow,
                ReferenceType = request.ReferenceType,
                ReferenceId = request.ReferenceId
            });

        await _context.SaveChangesAsync(cancellationToken);

        return new InventoryResponse(
            item.ProductId,
            item.QuantityOnHand,
            item.QuantityReserved,
            item.AvailableQuantity,
            item.ReorderLevel);
    }
}