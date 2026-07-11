using System.ComponentModel.DataAnnotations;

namespace TheNoir.Api.Dtos;

public class ToppingRequest
{
    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [Range(0, 500_000)]
    public int PriceVnd { get; set; }
}
