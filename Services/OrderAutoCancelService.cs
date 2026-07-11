using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

// Cancels any order still open 24h after creation (Order.ExpiresAt, fixed at
// creation time regardless of intervening status changes). Runs on a scoped
// DbContext per tick since BackgroundService itself is a singleton.
public class OrderAutoCancelService(IServiceScopeFactory scopeFactory, ILogger<OrderAutoCancelService> logger)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            await CancelExpiredOrdersAsync(stoppingToken);
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CancelExpiredOrdersAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var now = DateTime.UtcNow;
        var expired = await db.Orders
            .Include(o => o.City)
            .Where(o => o.Status != OrderStatuses.Completed && o.Status != OrderStatuses.Cancelled)
            .Where(o => o.ExpiresAt <= now)
            .ToListAsync(stoppingToken);

        if (expired.Count == 0) return;

        foreach (var order in expired)
        {
            order.Status = OrderStatuses.Cancelled;
            order.AutoCancelled = true;
            order.UpdatedAt = now;
        }

        await db.SaveChangesAsync(stoppingToken);
        logger.LogInformation("Auto-cancelled {Count} order(s) past their 24h deadline.", expired.Count);

        foreach (var order in expired.Where(o => o.UserId is not null))
        {
            var userEmail = await db.Users.Where(u => u.Id == order.UserId).Select(u => u.Email).FirstAsync(stoppingToken);
            await emailService.SendAsync(
                userEmail, $"Order #{order.Id} cancelled", BuildAutoCancelledHtml(order));
        }
    }

    private static string BuildAutoCancelledHtml(Order order) => $"""
        <p>Order #{order.Id} was automatically cancelled after 24 hours with no pickup or update.</p>
        <p>Total: {order.TotalVnd:N0}₫, pickup was set for {order.City?.Name}.</p>
        <p>If you'd still like this order, please place a new one.</p>
        """;
}
