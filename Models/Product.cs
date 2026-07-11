namespace TheNoir.Api.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    // Rich HTML from the admin's WYSIWYG editor, shown on the product detail
    // page only. Description above stays the short plain-text teaser used on
    // menu cards, cart lines and search, so this is optional and additive.
    public string? DescriptionHtml { get; set; }

    // Store the raw amount (65000); the frontend formats it as "65.000₫".
    public int PriceVnd { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Both default at write time when left blank: ImageUrl falls back to a
    // stock photo, ImageAlt mirrors the product name. See ProductService.
    public string? ImageUrl { get; set; }
    public string? ImageAlt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
