namespace TheNoir.Api.Models;

public class City
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int MaisonCount { get; set; }

    // e.g. Kind = "Flagship", Address = "42 Vườn Trà" — the frontend joins them for display.
    public required string Kind { get; set; }
    public required string Address { get; set; }

    // Display order in the homepage "Find Us" list — lower shows first.
    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
