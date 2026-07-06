using System.ComponentModel.DataAnnotations;

namespace TheNoir.Api.Dtos;

// Shared by POST (create) and PUT (update). The Id comes from the route, never the body.
public class ProductRequest
{
    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [Required, MaxLength(50)]
    public required string Category { get; set; }

    [Required, MaxLength(500)]
    public required string Description { get; set; }

    [Range(0, 10_000_000)]
    public int PriceVnd { get; set; }

    [Required, Url, MaxLength(500)]
    public required string ImageUrl { get; set; }

    [Required, MaxLength(200)]
    public required string ImageAlt { get; set; }

    public int SortOrder { get; set; }
}
