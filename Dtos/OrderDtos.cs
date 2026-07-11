using System.ComponentModel.DataAnnotations;
using TheNoir.Api.Models;

namespace TheNoir.Api.Dtos;

public class OrderItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 20)]
    public int Quantity { get; set; }

    [MaxLength(20)]
    public string? IceOption { get; set; }

    [MaxLength(10)]
    public string? Temperature { get; set; }

    [MaxLength(10)]
    public string? SugarLevel { get; set; }

    // Nullable at the DTO level since pastries don't take one; OrderService
    // rejects a missing Size for products whose category requires it.
    [MaxLength(20)]
    public string? Size { get; set; }

    [MaxLength(300)]
    public string? Note { get; set; }

    public List<int>? ToppingIds { get; set; }
}

public class CreateOrderRequest
{
    [Required]
    public int CityId { get; set; }

    [Required, MinLength(1)]
    public required List<OrderItemRequest> Items { get; set; }

    // Required for guest checkout only; ignored for logged-in users.
    [MaxLength(100)]
    public string? CustomerName { get; set; }
}

public class UpdateOrderStatusRequest
{
    [Required]
    public required string Status { get; set; }
}

public class CreateOrderMessageRequest
{
    [Required, MaxLength(300)]
    public required string Body { get; set; }
}

public record ToppingLineResponse(int? ToppingId, string Name, int PriceVnd)
{
    public static ToppingLineResponse From(OrderItemTopping t) =>
        new(t.ToppingId, t.ToppingName, t.ToppingPriceVnd);
}

public record OrderItemResponse(
    int? ProductId,
    string ProductName,
    int UnitPriceVnd,
    int Quantity,
    int LineTotalVnd,
    string? IceOption,
    string? Temperature,
    string? SugarLevel,
    string? Size,
    int SizeSurchargeVnd,
    string? Note,
    List<ToppingLineResponse> Toppings)
{
    public static OrderItemResponse From(OrderItem item) =>
        new(item.ProductId, item.ProductName, item.UnitPriceVnd, item.Quantity, item.LineTotalVnd,
            item.IceOption, item.Temperature, item.SugarLevel, item.Size, item.SizeSurchargeVnd, item.Note,
            item.Toppings.Select(ToppingLineResponse.From).ToList());
}

public record OrderResponse(
    int Id,
    string Status,
    int CityId,
    string CityName,
    int TotalVnd,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime ExpiresAt,
    bool AutoCancelled,
    string? CustomerName,
    // False for guest checkouts: there's no account to sign back in with, so
    // the chat thread only reaches one way and the admin UI hides it.
    bool HasAccount,
    DateTime? PreparingAt,
    DateTime? ReadyAt,
    DateTime? CompletedAt,
    DateTime? CancelledAt,
    List<OrderItemResponse> Items)
{
    // CustomerName is a raw column that only guest checkouts populate; for
    // a logged-in order it's null, so fall back to the account's DisplayName.
    public static OrderResponse From(Order order) => new(
        order.Id,
        order.Status,
        order.CityId,
        order.City?.Name ?? "",
        order.TotalVnd,
        order.CreatedAt,
        order.UpdatedAt,
        order.ExpiresAt,
        order.AutoCancelled,
        order.CustomerName ?? order.User?.DisplayName,
        order.UserId is not null,
        order.PreparingAt,
        order.ReadyAt,
        order.CompletedAt,
        order.CancelledAt,
        order.Items.Select(OrderItemResponse.From).ToList());
}

public record OrderMessageResponse(int Id, string SenderRole, string Body, DateTime CreatedAt)
{
    public static OrderMessageResponse From(OrderMessage m) => new(m.Id, m.SenderRole, m.Body, m.CreatedAt);
}

public record UnseenOrdersSummary(int NewOrders, int AutoCancelled);
