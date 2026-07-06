using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;

namespace TheNoir.Api.Features.Maisons;

public static class MaisonEndpoints
{
    public static IEndpointRouteBuilder MapMaisons(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/maisons", async (AppDbContext db) =>
            {
                var cities = await db.Cities.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync();
                return new MaisonsResponse(cities.Sum(c => c.MaisonCount), cities);
            })
            .WithName("GetMaisons");

        return app;
    }
}

public record MaisonsResponse(int TotalMaisons, IReadOnlyList<City> Cities);
