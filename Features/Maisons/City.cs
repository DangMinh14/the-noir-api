namespace TheNoir.Api.Features.Maisons;

public class City
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int MaisonCount { get; set; }

    // e.g. Kind = "Flagship", Address = "42 Vườn Trà" — the frontend joins them for display.
    public required string Kind { get; set; }
    public required string Address { get; set; }

    public int SortOrder { get; set; }
}
