using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class OrderService(AppDbContext db, IEmailService emailService) : IOrderService
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

        var userEmail = await db.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstAsync();
        await emailService.SendAsync(userEmail, $"Order #{order.Id} confirmed", BuildOrderConfirmationHtml(order));

        return ServiceResult<OrderResponse>.Ok(OrderResponse.From(order));
    }

    private static string BuildOrderConfirmationHtml(Order order)
    {
        var rows = string.Join("", order.Items.Select(i =>
            $"<tr><td>{i.ProductName} x{i.Quantity}</td><td style=\"text-align:right\">{i.LineTotalVnd:N0}₫</td></tr>"));

        return $"""
            <p>Thanks for your order! Here's what's coming:</p>
            <table style="width:100%;border-collapse:collapse">{rows}</table>
            <p><strong>Total: {order.TotalVnd:N0}₫</strong></p>
            <p>Pickup at {order.City?.Name}.</p>
            """;
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

    public async Task<PagedResult<OrderResponse>> GetAllAsync(string? search, int page, int pageSize)
    {
        var query = db.Orders.AsNoTracking().Include(o => o.Items).Include(o => o.City).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(o => o.Id.ToString() == term ||
                                      EF.Functions.Like(o.Status, $"%{term}%") ||
                                      (o.City != null && EF.Functions.Like(o.City.Name, $"%{term}%")));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OrderResponse>(items.Select(OrderResponse.From).ToList(), totalCount, page, pageSize);
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
