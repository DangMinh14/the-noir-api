using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class OrderService(AppDbContext db, IEmailService emailService) : IOrderService
{
    public async Task<ServiceResult<OrderResponse>> CreateAsync(int? userId, CreateOrderRequest request)
    {
        var customerName = request.CustomerName?.Trim();
        if (userId is null && string.IsNullOrEmpty(customerName))
            return ServiceResult<OrderResponse>.Fail("Enter your name, or log in, to place an order.");

        if (!await db.Cities.AnyAsync(c => c.Id == request.CityId))
            return ServiceResult<OrderResponse>.Fail("Selected maison does not exist.");

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products
            .Include(p => p.Category)
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var missingIds = productIds.Except(products.Keys).ToList();
        if (missingIds.Count > 0)
            return ServiceResult<OrderResponse>.Fail($"Product {missingIds[0]} no longer exists.");

        var toppingIds = request.Items.SelectMany(i => i.ToppingIds ?? []).Distinct().ToList();
        var toppings = await db.Toppings
            .Where(t => toppingIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id);

        var missingToppingIds = toppingIds.Except(toppings.Keys).ToList();
        if (missingToppingIds.Count > 0)
            return ServiceResult<OrderResponse>.Fail($"Topping {missingToppingIds[0]} no longer exists.");

        foreach (var itemRequest in request.Items)
        {
            var product = products[itemRequest.ProductId];

            // AllowsToppings doubles as "this is a drink": everything below
            // (ice/temperature/sugar/size/toppings) is drink customization,
            // so a category that opts out of one opts out of all of them.
            // Non-drink products (pastries, standalone toppings) get only
            // quantity + note.
            if (!product.Category!.AllowsToppings)
            {
                if (itemRequest.IceOption is not null || itemRequest.Temperature is not null ||
                    itemRequest.SugarLevel is not null || !string.IsNullOrEmpty(itemRequest.Size) ||
                    itemRequest.ToppingIds is { Count: > 0 })
                    return ServiceResult<OrderResponse>.Fail($"{product.Name} doesn't take drink customization.");
                continue;
            }

            if (itemRequest.IceOption is not null && !IceOptions.All.Contains(itemRequest.IceOption))
                return ServiceResult<OrderResponse>.Fail($"Ice option must be one of: {string.Join(", ", IceOptions.All)}.");
            if (itemRequest.Temperature is not null && !Temperatures.All.Contains(itemRequest.Temperature))
                return ServiceResult<OrderResponse>.Fail($"Temperature must be one of: {string.Join(", ", Temperatures.All)}.");
            if (itemRequest.SugarLevel is not null && !SugarLevels.All.Contains(itemRequest.SugarLevel))
                return ServiceResult<OrderResponse>.Fail($"Sugar level must be one of: {string.Join(", ", SugarLevels.All)}.");
            if (string.IsNullOrEmpty(itemRequest.Size) || !Sizes.All.Contains(itemRequest.Size))
                return ServiceResult<OrderResponse>.Fail($"{product.Name} needs a size: {string.Join(", ", Sizes.All)}.");
        }

        var now = DateTime.UtcNow;
        var order = new Order
        {
            UserId = userId,
            CustomerName = userId is null ? customerName : null,
            CityId = request.CityId,
            Status = OrderStatuses.Pending,
            CreatedAt = now,
            UpdatedAt = now,
            ExpiresAt = now.AddHours(24),
        };

        foreach (var itemRequest in request.Items)
        {
            var product = products[itemRequest.ProductId];
            var itemToppings = (itemRequest.ToppingIds ?? [])
                .Select(id => toppings[id])
                .Select(t => new OrderItemTopping
                {
                    ToppingId = t.Id,
                    ToppingName = t.Name,
                    ToppingPriceVnd = t.PriceVnd,
                })
                .ToList();
            var toppingsSum = itemToppings.Sum(t => t.ToppingPriceVnd);
            var sizeSurcharge = itemRequest.Size == Sizes.Large ? Sizes.LargeSurchargeFor(product.PriceVnd) : 0;
            var lineTotal = (product.PriceVnd + sizeSurcharge + toppingsSum) * itemRequest.Quantity;
            var note = itemRequest.Note?.Trim();
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPriceVnd = product.PriceVnd,
                Quantity = itemRequest.Quantity,
                LineTotalVnd = lineTotal,
                IceOption = itemRequest.IceOption,
                Temperature = itemRequest.Temperature,
                SugarLevel = itemRequest.SugarLevel,
                Size = itemRequest.Size,
                SizeSurchargeVnd = sizeSurcharge,
                Note = string.IsNullOrEmpty(note) ? null : note,
                Toppings = itemToppings,
            });
        }

        order.TotalVnd = order.Items.Sum(i => i.LineTotalVnd);

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        await db.Entry(order).Reference(o => o.City).LoadAsync();
        if (userId is not null) await db.Entry(order).Reference(o => o.User).LoadAsync();

        if (userId is not null)
        {
            var userEmail = await db.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstAsync();
            await emailService.SendAsync(userEmail, $"Order #{order.Id} confirmed", BuildOrderConfirmationHtml(order));
        }

        return ServiceResult<OrderResponse>.Ok(OrderResponse.From(order));
    }

    private static string BuildOrderConfirmationHtml(Order order)
    {
        var rows = string.Join("", order.Items.Select(i =>
        {
            var toppingNote = i.Toppings.Count > 0
                ? $"<br><small>{string.Join(", ", i.Toppings.Select(t => t.ToppingName))}</small>"
                : "";
            return $"<tr><td>{i.ProductName} x{i.Quantity}{toppingNote}</td><td style=\"text-align:right\">{i.LineTotalVnd:N0}₫</td></tr>";
        }));

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
            .Include(o => o.Items).ThenInclude(i => i.Toppings)
            .Include(o => o.City)
            .Include(o => o.User)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(OrderResponse.From).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(int id, int requestingUserId, bool isAdmin)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Toppings)
            .Include(o => o.City)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return null;
        if (!isAdmin && order.UserId != requestingUserId) return null;

        return OrderResponse.From(order);
    }

    public async Task<PagedResult<OrderResponse>> GetAllAsync(
        string? search, string? status, int? cityId, bool? autoCancelled,
        DateTime? from, DateTime? to, int page, int pageSize)
    {
        var query = db.Orders.AsNoTracking()
            .Include(o => o.Items).ThenInclude(i => i.Toppings)
            .Include(o => o.City)
            .Include(o => o.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(o => o.Id.ToString() == term ||
                                      EF.Functions.Like(o.Status, $"%{term}%") ||
                                      (o.City != null && EF.Functions.Like(o.City.Name, $"%{term}%")));
        }

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);

        if (cityId is not null)
            query = query.Where(o => o.CityId == cityId);

        if (autoCancelled is not null)
            query = query.Where(o => o.AutoCancelled == autoCancelled);

        if (from is not null)
            query = query.Where(o => o.CreatedAt >= from);

        if (to is not null)
            query = query.Where(o => o.CreatedAt <= to);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OrderResponse>(items.Select(OrderResponse.From).ToList(), totalCount, page, pageSize);
    }

    public async Task<UnseenOrdersSummary> GetUnseenSummaryAsync(DateTime since)
    {
        var newOrders = await db.Orders.CountAsync(o => o.CreatedAt > since);
        var autoCancelled = await db.Orders.CountAsync(o => o.AutoCancelled && o.UpdatedAt > since);
        return new UnseenOrdersSummary(newOrders, autoCancelled);
    }

    public async Task<ServiceResult<OrderResponse>> UpdateStatusAsync(int id, string status)
    {
        if (!OrderStatuses.All.Contains(status))
            return ServiceResult<OrderResponse>.Fail($"Status must be one of: {string.Join(", ", OrderStatuses.All)}.");

        var order = await db.Orders
            .Include(o => o.Items).ThenInclude(i => i.Toppings)
            .Include(o => o.City)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            return ServiceResult<OrderResponse>.Fail("Order not found.");

        if (order.Status is OrderStatuses.Completed or OrderStatuses.Cancelled)
            return ServiceResult<OrderResponse>.Fail($"This order is already {order.Status}.");

        ApplyStatusTimestamps(order, status);
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return ServiceResult<OrderResponse>.Ok(OrderResponse.From(order));
    }

    // Stamps the timestamp for the step being entered and clears any step
    // timestamps beyond it, so stepping back (e.g. Ready -> Preparing) then
    // re-advancing records a fresh time instead of the stale earlier one.
    private static void ApplyStatusTimestamps(Order order, string newStatus)
    {
        var now = DateTime.UtcNow;

        if (newStatus == OrderStatuses.Cancelled)
        {
            order.CancelledAt = now;
            return;
        }

        switch (newStatus)
        {
            case OrderStatuses.Pending:
                order.PreparingAt = null;
                order.ReadyAt = null;
                order.CompletedAt = null;
                break;
            case OrderStatuses.Preparing:
                order.PreparingAt = now;
                order.ReadyAt = null;
                order.CompletedAt = null;
                break;
            case OrderStatuses.Ready:
                order.PreparingAt ??= now;
                order.ReadyAt = now;
                order.CompletedAt = null;
                break;
            case OrderStatuses.Completed:
                order.PreparingAt ??= now;
                order.ReadyAt ??= now;
                order.CompletedAt = now;
                break;
        }
    }

    public async Task<ServiceResult<List<OrderMessageResponse>>> GetMessagesAsync(int orderId, int requestingUserId, bool isAdminOrStaff)
    {
        var order = await db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null || (!isAdminOrStaff && order.UserId != requestingUserId))
            return ServiceResult<List<OrderMessageResponse>>.Fail("Order not found.");

        var messages = await db.OrderMessages
            .AsNoTracking()
            .Where(m => m.OrderId == orderId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return ServiceResult<List<OrderMessageResponse>>.Ok(messages.Select(OrderMessageResponse.From).ToList());
    }

    public async Task<ServiceResult<OrderMessageResponse>> AddMessageAsync(int orderId, int requestingUserId, bool isAdminOrStaff, string body)
    {
        var trimmed = body.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return ServiceResult<OrderMessageResponse>.Fail("Message can't be empty.");

        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order is null) return ServiceResult<OrderMessageResponse>.Fail("Order not found.");

        // Ownership decides the label, not role: an account can hold Staff/Admin
        // and still be the customer on its own order (e.g. a staff member's
        // personal order), so posting there is always as the customer.
        var isOwner = order.UserId == requestingUserId;
        if (!isAdminOrStaff && !isOwner)
            return ServiceResult<OrderMessageResponse>.Fail("Order not found.");

        var message = new OrderMessage
        {
            OrderId = orderId,
            SenderRole = isOwner ? OrderMessageSenders.Customer : OrderMessageSenders.Staff,
            Body = trimmed,
            CreatedAt = DateTime.UtcNow,
        };
        db.OrderMessages.Add(message);
        await db.SaveChangesAsync();

        return ServiceResult<OrderMessageResponse>.Ok(OrderMessageResponse.From(message));
    }
}
