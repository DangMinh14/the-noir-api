using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardStatsResponse> GetStatsAsync()
    {
        // Small data volume expected for this project, so aggregate in
        // memory rather than fight SQLite's limited date-function
        // translation in EF Core LINQ-to-SQL.
        var orders = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.Status != OrderStatuses.Cancelled)
            .ToListAsync();

        var totalRevenue = orders.Sum(o => o.TotalVnd);
        var totalOrders = await db.Orders.CountAsync();

        var since = DateTime.UtcNow.Date.AddDays(-13);
        var revenueByDay = Enumerable.Range(0, 14)
            .Select(offset => since.AddDays(offset))
            .Select(date => new DailyRevenue(
                DateOnly.FromDateTime(date),
                orders.Where(o => o.CreatedAt.Date == date).Sum(o => o.TotalVnd)))
            .ToList();

        var topProducts = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductName)
            .Select(g => new ProductSales(g.Key, g.Sum(i => i.Quantity), g.Sum(i => i.LineTotalVnd)))
            .OrderByDescending(p => p.QuantitySold)
            .Take(5)
            .ToList();

        var productCategories = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ToDictionaryAsync(p => p.Id, p => p.Category?.Name ?? "Uncategorized");

        var revenueByCategory = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductId.HasValue && productCategories.TryGetValue(i.ProductId.Value, out var name)
                ? name
                : "Uncategorized")
            .Select(g => new CategoryRevenue(g.Key, g.Sum(i => i.LineTotalVnd)))
            .OrderByDescending(c => c.RevenueVnd)
            .ToList();

        return new DashboardStatsResponse(totalRevenue, totalOrders, revenueByDay, topProducts, revenueByCategory);
    }
}
