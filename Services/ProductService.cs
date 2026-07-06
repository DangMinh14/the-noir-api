using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class ProductService(AppDbContext db) : IProductService
{
    // Shown when a product has no photo yet, so the catalog never has a
    // broken <img>. A real photo can be uploaded any time afterwards.
    // Served from wwwroot/branding (not wwwroot/uploads, which is gitignored
    // per-environment) so it ships with the repo like any other brand asset.
    public const string FallbackImageUrl = "/branding/product-fallback.jpg";

    public async Task<List<ProductResponse>> GetAllAsync()
    {
        var products = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();

        return products.Select(ProductResponse.From).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product is null ? null : ProductResponse.From(product);
    }

    public async Task<ServiceResult<ProductResponse>> CreateAsync(ProductRequest request)
    {
        if (!await db.Categories.AnyAsync(c => c.Id == request.CategoryId))
            return ServiceResult<ProductResponse>.Fail("Selected category does not exist.");

        var now = DateTime.UtcNow;
        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            PriceVnd = request.PriceVnd,
            CategoryId = request.CategoryId,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? FallbackImageUrl : request.ImageUrl.Trim(),
            ImageAlt = string.IsNullOrWhiteSpace(request.ImageAlt) ? request.Name.Trim() : request.ImageAlt.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();
        await db.Entry(product).Reference(p => p.Category).LoadAsync();
        return ServiceResult<ProductResponse>.Ok(ProductResponse.From(product));
    }

    public async Task<ServiceResult<ProductResponse>> UpdateAsync(int id, ProductRequest request)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null)
            return ServiceResult<ProductResponse>.Fail("Product not found.");

        if (!await db.Categories.AnyAsync(c => c.Id == request.CategoryId))
            return ServiceResult<ProductResponse>.Fail("Selected category does not exist.");

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.PriceVnd = request.PriceVnd;
        product.CategoryId = request.CategoryId;
        product.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? FallbackImageUrl : request.ImageUrl.Trim();
        product.ImageAlt = string.IsNullOrWhiteSpace(request.ImageAlt) ? request.Name.Trim() : request.ImageAlt.Trim();
        product.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        await db.Entry(product).Reference(p => p.Category).LoadAsync();
        return ServiceResult<ProductResponse>.Ok(ProductResponse.From(product));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return false;

        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return true;
    }
}
