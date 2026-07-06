using System.ComponentModel.DataAnnotations;
using TheNoir.Api.Models;

namespace TheNoir.Api.Dtos;

public class OrderItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 20)]
    public int Quantity { get; set; }
}

public class CreateOrderRequest
{
    [Required]
    public int CityId { get; set; }

    [Required, MinLength(1)]
    public required List<OrderItemRequest> Items { get; set; }
}

public class UpdateOrderStatusRequest
{
    [Required]
    public required string Status { get; set; }
}

public record OrderItemResponse(
    int? ProductId,
    string ProductName,
    int UnitPriceVnd,
    int Quantity,
    int LineTotalVnd)
{
    public static OrderItemResponse From(OrderItem item) =>
        new(item.ProductId, item.ProductName, item.UnitPriceVnd, item.Quantity, item.LineTotalVnd);
}

public record OrderResponse(
    int Id,
    string Status,
    int CityId,
    string CityName,
    int TotalVnd,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<OrderItemResponse> Items)
{
    public static OrderResponse From(Order order) => new(
        order.Id,
        order.Status,
        order.CityId,
        order.City?.Name ?? "",
        order.TotalVnd,
        order.CreatedAt,
        order.UpdatedAt,
        order.Items.Select(OrderItemResponse.From).ToList());
}
