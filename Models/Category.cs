namespace TheNoir.Api.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Product> Products { get; set; } = [];
}
