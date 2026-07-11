namespace TheNoir.Api.Models;

public static class IceOptions
{
    public const string OnTheSide = "Ice on the side";
    public const string Mixed = "Ice mixed in";
    public const string Less = "Less ice";
    public const string None = "No ice";

    public static readonly string[] All = [OnTheSide, Mixed, Less, None];
}

public static class Temperatures
{
    public const string Hot = "Hot";
    public const string Cold = "Cold";

    public static readonly string[] All = [Hot, Cold];
}

public static class SugarLevels
{
    public const string Full = "100% sugar";
    public const string Seventy = "70% sugar";
    public const string Half = "50% sugar";
    public const string Thirty = "30% sugar";
    public const string None = "No sugar";

    public static readonly string[] All = [Full, Seventy, Half, Thirty, None];
}

// Unlike Ice/Temperature/Sugar, Size is mandatory for drinks (see
// OrderService.CreateAsync) and carries a price: Large adds a surcharge
// computed from the product's own base price, so a 55k coffee and an 85k
// specialty both upsize by a proportional, sane-looking amount instead of
// one flat number that's wrong at either end.
public static class Sizes
{
    public const string Medium = "M · 500ml";
    public const string Large = "L · 700ml";

    public static readonly string[] All = [Medium, Large];

    private const decimal LargeSurchargeRate = 0.15m;

    public static int LargeSurchargeFor(int basePriceVnd) =>
        (int)Math.Round(basePriceVnd * LargeSurchargeRate / 1000m, MidpointRounding.AwayFromZero) * 1000;
}

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

    // All optional: absence means the shop's standard preparation, not an error.
    public string? IceOption { get; set; }
    public string? Temperature { get; set; }
    public string? SugarLevel { get; set; }
    public string? Note { get; set; }

    // Null for products whose category doesn't take a size (pastries); the
    // service enforces it's set for everything else. Surcharge is
    // snapshotted like UnitPriceVnd so a later change to the surcharge rate
    // never rewrites a past receipt.
    public string? Size { get; set; }
    public int SizeSurchargeVnd { get; set; }

    public List<OrderItemTopping> Toppings { get; set; } = [];
}

// Join/snapshot row: a topping selected on an order item, with name/price
// snapshotted at order time (same reasoning as OrderItem.ProductName/UnitPriceVnd).
public class OrderItemTopping
{
    public int Id { get; set; }
    public int OrderItemId { get; set; }
    public OrderItem? OrderItem { get; set; }

    // Nullable: the topping may be deleted later without breaking this row.
    public int? ToppingId { get; set; }
    public Topping? Topping { get; set; }

    public required string ToppingName { get; set; }
    public int ToppingPriceVnd { get; set; }
}
