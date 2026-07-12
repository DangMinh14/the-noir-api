using System.ComponentModel.DataAnnotations;
using TheNoir.Api.Models;

namespace TheNoir.Api.Dtos;

public class CategoryRequest
{
    [Required, MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(280)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(200)]
    public string? ImageAlt { get; set; }

    public bool AllowsToppings { get; set; } = true;
}

public record CategoryResponse(
    int Id,
    string Name,
    string Description,
    int ProductCount,
    string ImageUrl,
    string ImageAlt,
    bool AllowsToppings,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static CategoryResponse From(Category c) =>
        new(c.Id, c.Name, c.Description ?? "", c.Products.Count, c.ImageUrl ?? "", c.ImageAlt ?? "", c.AllowsToppings, c.CreatedAt, c.UpdatedAt);
}
