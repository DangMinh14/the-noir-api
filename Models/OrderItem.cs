namespace TheNoir.Api.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    // Nullable: the product may be deleted later without breaking this row.
    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    // Snapshotted at order time so a later price/name edit never rewrites
    // what a customer's receipt already showed.
    public required string ProductName { get; set; }
    public int UnitPriceVnd { get; set; }

    public int Quantity { get; set; }
    public int LineTotalVnd { get; set; }
}
