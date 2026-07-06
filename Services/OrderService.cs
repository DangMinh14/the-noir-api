using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class OrderService(AppDbContext db) : IOrderService
{
    public async Task<ServiceResult<OrderResponse>> CreateAsync(int userId, CreateOrderRequest request)
    {
        if (!await db.Cities.AnyAsync(c => c.Id == request.CityId))
            return ServiceResult<OrderResponse>.Fail("Selected maison does not exist.");

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var missing = productIds.Except(products.Keys).FirstOrDefault();
        if (missing != 0)
            return ServiceResult<OrderResponse>.Fail($"Product {missing} no longer exists.");

        var now = DateTime.UtcNow;
        var order = new Order
        {
            UserId = userId,
            CityId = request.CityId,
            Status = OrderStatuses.Pending,
            CreatedAt = now,
            UpdatedAt = now,
        };

        foreach (var itemRequest in request.Items)
        {
            var product = products[itemRequest.ProductId];
            var lineTotal = product.PriceVnd * itemRequest.Quantity;
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPriceVnd = product.PriceVnd,
                Quantity = itemRequest.Quantity,
                LineTotalVnd = lineTotal,
            });
        }

        order.TotalVnd = order.Items.Sum(i => i.LineTotalVnd);

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        await db.Entry(order).Reference(o => o.City).LoadAsync();

        return ServiceResult<OrderResponse>.Ok(OrderResponse.From(order));
    }

    public async Task<List<OrderResponse>> GetMineAsync(int userId)
    {
        var orders = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.City)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(OrderResponse.From).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(int id, int requestingUserId, bool isAdmin)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.City)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return null;
        if (!isAdmin && order.UserId != requestingUserId) return null;

        return OrderResponse.From(order);
    }

    public async Task<List<OrderResponse>> GetAllAsync()
    {
        var orders = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Include(o => o.City)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(OrderResponse.From).ToList();
    }

    public async Task<ServiceResult<OrderResponse>> UpdateStatusAsync(int id, string status)
    {
        if (!OrderStatuses.All.Contains(status))
            return ServiceResult<OrderResponse>.Fail($"Status must be one of: {string.Join(", ", OrderStatuses.All)}.");

        var order = await db.Orders
            .Include(o => o.Items)
            .Include(o => o.City)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            return ServiceResult<OrderResponse>.Fail("Order not found.");

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return ServiceResult<OrderResponse>.Ok(OrderResponse.From(order));
    }
}
