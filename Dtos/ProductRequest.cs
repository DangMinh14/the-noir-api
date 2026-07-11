using System.ComponentModel.DataAnnotations;
using TheNoir.Api.Models;

namespace TheNoir.Api.Dtos;

// Shared by POST (create) and PUT (update). The Id comes from the route, never the body.
public class ProductRequest
{
    [Required, MaxLength(100)]
    public required string Name { get; set; }

    public int CategoryId { get; set; }

    [Required, MaxLength(500)]
    public required string Description { get; set; }

    // Optional rich HTML from the admin's WYSIWYG editor, rendered on the
    // product detail page. Sanitized client-side (sanitize-html.ts) both
    // before this is sent and again before it's rendered with
    // dangerouslySetInnerHTML, since admin sessions aren't a substitute
    // for not trusting stored markup.
    [MaxLength(20_000)]
    public string? DescriptionHtml { get; set; }

    [Range(0, 10_000_000)]
    public int PriceVnd { get; set; }

    // Optional: blank falls back to a stock photo (see ProductService).
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    // Optional: blank defaults to the product name.
    [MaxLength(200)]
    public string? ImageAlt { get; set; }
}

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    string DescriptionHtml,
    int PriceVnd,
    int CategoryId,
    string CategoryName,
    bool CategoryAllowsToppings,
    string ImageUrl,
    string ImageAlt,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static ProductResponse From(Product p) => new(
        p.Id,
        p.Name,
        p.Description,
        p.DescriptionHtml ?? "",
        p.PriceVnd,
        p.CategoryId,
        p.Category?.Name ?? "",
        p.Category?.AllowsToppings ?? true,
        p.ImageUrl ?? "",
        p.ImageAlt ?? "",
        p.CreatedAt,
        p.UpdatedAt);
}
