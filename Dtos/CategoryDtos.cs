using System.ComponentModel.DataAnnotations;
using TheNoir.Api.Models;

namespace TheNoir.Api.Dtos;

public class CategoryRequest
{
    [Required, MaxLength(50)]
    public required string Name { get; set; }
}

public record CategoryResponse(int Id, string Name, int ProductCount, DateTime CreatedAt, DateTime UpdatedAt)
{
    public static CategoryResponse From(Category c) =>
        new(c.Id, c.Name, c.Products.Count, c.CreatedAt, c.UpdatedAt);
}
