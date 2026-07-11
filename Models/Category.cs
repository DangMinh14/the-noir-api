namespace TheNoir.Api.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Nullable: blank means the frontend falls back to a stock photo, same
    // convention as Product.ImageAlt (no fallback baked into this column).
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // False for food items (e.g. pastries) that don't take drink toppings.
    public bool AllowsToppings { get; set; } = true;

    public List<Product> Products { get; set; } = [];
}
