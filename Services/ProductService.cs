using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class ProductService(AppDbContext db) : IProductService
{
    public Task<List<Product>> GetAllAsync() =>
        db.Products.AsNoTracking().OrderBy(p => p.SortOrder).ToListAsync();

    public Task<Product?> GetByIdAsync(int id) =>
        db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Product> CreateAsync(ProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Category = request.Category,
            Description = request.Description,
            PriceVnd = request.PriceVnd,
            ImageUrl = request.ImageUrl,
            ImageAlt = request.ImageAlt,
            SortOrder = request.SortOrder,
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, ProductRequest request)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return null;

        product.Name = request.Name;
        product.Category = request.Category;
        product.Description = request.Description;
        product.PriceVnd = request.PriceVnd;
        product.ImageUrl = request.ImageUrl;
        product.ImageAlt = request.ImageAlt;
        product.SortOrder = request.SortOrder;

        await db.SaveChangesAsync();
        return product;
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
