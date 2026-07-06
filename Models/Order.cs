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
    public int UserId { get; set; }
    public User? User { get; set; }

    // Pickup maison.
    public int CityId { get; set; }
    public City? City { get; set; }

    public string Status { get; set; } = OrderStatuses.Pending;

    // Sum of OrderItem.LineTotalVnd, computed server-side at creation.
    public int TotalVnd { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}
