using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;

namespace TheNoir.Api.Features.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProducts(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (AppDbContext db) =>
                await db.Products.AsNoTracking().OrderBy(p => p.SortOrder).ToListAsync())
            .WithName("GetProducts");

        return app;
    }
}
