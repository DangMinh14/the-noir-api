namespace TheNoir.Api.Features.Products;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required string Description { get; set; }

    // Store the raw amount (65000); the frontend formats it as "65.000₫".
    public int PriceVnd { get; set; }

    public required string ImageUrl { get; set; }
    public required string ImageAlt { get; set; }
    public int SortOrder { get; set; }
}
