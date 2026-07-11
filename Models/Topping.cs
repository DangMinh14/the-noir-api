namespace TheNoir.Api.Models;

public class Topping
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int PriceVnd { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
