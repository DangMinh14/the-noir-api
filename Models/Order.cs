namespace TheNoir.Api.Models;

public static class OrderStatuses
{
    public const string Pending = "Pending";
    public const string Preparing = "Preparing";
    public const string Ready = "Ready";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All = [Pending, Preparing, Ready, Completed, Cancelled];
}

public class Order
{
    public int Id { get; set; }

    // Null for guest checkout — CustomerName is used to identify the order instead.
    public int? UserId { get; set; }
    public User? User { get; set; }

    // Only set for guest orders (UserId is null).
    public string? CustomerName { get; set; }

    // Pickup maison.
    public int CityId { get; set; }
    public City? City { get; set; }

    public string Status { get; set; } = OrderStatuses.Pending;

    // Sum of OrderItem.LineTotalVnd, computed server-side at creation.
    public int TotalVnd { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Fixed at creation (CreatedAt + 24h); OrderAutoCancelService cancels
    // the order once this passes, regardless of intervening status changes.
    public DateTime ExpiresAt { get; set; }

    // True only when OrderAutoCancelService cancelled the order, so the
    // admin UI can tell that apart from a deliberate manual cancel.
    public bool AutoCancelled { get; set; }

    // Stamped the first time the order enters each step (OrderService keeps
    // these in sync when staff step back and re-advance), so the admin and
    // customer UIs can show a real timeline instead of just CreatedAt/UpdatedAt.
    public DateTime? PreparingAt { get; set; }
    public DateTime? ReadyAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public List<OrderItem> Items { get; set; } = [];
    public List<OrderMessage> Messages { get; set; } = [];
}
