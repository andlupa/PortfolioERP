using Microsoft.EntityFrameworkCore;
using PortfolioERP.Application.Features.Dashboard;
using PortfolioERP.Domain.Enums;
using PortfolioERP.Infrastructure.Persistence;

namespace PortfolioERP.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly AppDbContext _dbContext;

    public DashboardService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardResponse> GetAsync(
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var monthStart = new DateTime(
            now.Year,
            now.Month,
            1,
            0,
            0,
            0,
            DateTimeKind.Utc);

        var nextMonthStart = monthStart.AddMonths(1);

        var activeProducts = await _dbContext.Products
            .AsNoTracking()
            .CountAsync(
                product => product.IsActive,
                cancellationToken);

        var activeCustomers = await _dbContext.Customers
            .AsNoTracking()
            .CountAsync(
                customer => customer.IsActive,
                cancellationToken);

        var totalOrders = await _dbContext.SalesOrders
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var ordersThisMonth = await _dbContext.SalesOrders
            .AsNoTracking()
            .CountAsync(
                order =>
                    order.OrderDate >= monthStart &&
                    order.OrderDate < nextMonthStart,
                cancellationToken);

        var revenueThisMonth = await _dbContext.SalesOrders
            .AsNoTracking()
            .Where(order =>
                order.OrderDate >= monthStart &&
                order.OrderDate < nextMonthStart &&
                order.Status != OrderStatus.Cancelled)
            .SumAsync(
                order => (decimal?)order.TotalAmount,
                cancellationToken)
            ?? 0m;

        var ordersByStatusData = await _dbContext.SalesOrders
            .AsNoTracking()
            .GroupBy(order => order.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .OrderBy(item => item.Status)
            .ToListAsync(cancellationToken);

        var ordersByStatus = ordersByStatusData
            .Select(item =>
                new OrderStatusSummaryResponse(
                    item.Status,
                    item.Count))
            .ToList();

        var recentOrders = await _dbContext.SalesOrders
            .AsNoTracking()
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.Id)
            .Take(5)
            .Select(order =>
                new RecentOrderResponse(
                    order.Id,
                    order.OrderNumber,
                    order.OrderDate,
                    order.Customer.CompanyName,
                    order.Status,
                    order.TotalAmount))
            .ToListAsync(cancellationToken);

        var lowStockProducts = await _dbContext.Products
            .AsNoTracking()
            .Where(product =>
                product.IsActive &&
                product.StockQuantity <= 5)
            .OrderBy(product => product.StockQuantity)
            .ThenBy(product => product.Name)
            .Take(10)
            .Select(product =>
                new LowStockProductResponse(
                    product.Id,
                    product.Code,
                    product.Name,
                    product.StockQuantity))
            .ToListAsync(cancellationToken);

        var sixMonthsStart = monthStart.AddMonths(-5);

        var monthlyRevenueData = await _dbContext.SalesOrders
            .AsNoTracking()
            .Where(order =>
                order.OrderDate >= sixMonthsStart &&
                order.OrderDate < nextMonthStart &&
                order.Status != OrderStatus.Cancelled)
            .GroupBy(order => new
            {
                order.OrderDate.Year,
                order.OrderDate.Month
            })
            .Select(group => new
            {
                group.Key.Year,
                group.Key.Month,
                Revenue = group.Sum(order => order.TotalAmount)
            })
            .OrderBy(item => item.Year)
            .ThenBy(item => item.Month)
            .ToListAsync(cancellationToken);

        var monthlyRevenue = Enumerable
            .Range(0, 6)
            .Select(index => sixMonthsStart.AddMonths(index))
            .Select(date =>
            {
                var item = monthlyRevenueData.FirstOrDefault(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month);

                return new MonthlyRevenueResponse(
                    date.Year,
                    date.Month,
                    item?.Revenue ?? 0m);
            })
            .ToList();

        return new DashboardResponse(
            activeProducts,
            activeCustomers,
            totalOrders,
            ordersThisMonth,
            revenueThisMonth,
            ordersByStatus,
            recentOrders,
            lowStockProducts,
            monthlyRevenue);
    }
}