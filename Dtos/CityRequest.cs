using System.ComponentModel.DataAnnotations;

namespace TheNoir.Api.Dtos;

public class CityRequest
{
    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [Range(0, 1000)]
    public int MaisonCount { get; set; }

    [Required, MaxLength(50)]
    public required string Kind { get; set; }

    [Required, MaxLength(200)]
    public required string Address { get; set; }

    public int SortOrder { get; set; }
}
