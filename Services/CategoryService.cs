using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class CategoryService(AppDbContext db) : ICategoryService
{
    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories = await db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return categories.Select(CategoryResponse.From).ToList();
    }

    public async Task<PagedResult<CategoryResponse>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = db.Categories.AsNoTracking().Include(c => c.Products).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{term}%"));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<CategoryResponse>(items.Select(CategoryResponse.From).ToList(), totalCount, page, pageSize);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        return category is null ? null : CategoryResponse.From(category);
    }

    public async Task<ServiceResult<CategoryResponse>> CreateAsync(CategoryRequest request)
    {
        var name = request.Name.Trim();
        if (await db.Categories.AnyAsync(c => c.Name == name))
            return ServiceResult<CategoryResponse>.Fail("A category with this name already exists.");

        var now = DateTime.UtcNow;
        var category = new Category
        {
            Name = name,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
            ImageAlt = string.IsNullOrWhiteSpace(request.ImageAlt) ? null : request.ImageAlt.Trim(),
            AllowsToppings = request.AllowsToppings,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return ServiceResult<CategoryResponse>.Ok(CategoryResponse.From(category));
    }

    public async Task<ServiceResult<CategoryResponse>> UpdateAsync(int id, CategoryRequest request)
    {
        var category = await db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
            return ServiceResult<CategoryResponse>.Fail("Category not found.");

        var name = request.Name.Trim();
        if (await db.Categories.AnyAsync(c => c.Name == name && c.Id != id))
            return ServiceResult<CategoryResponse>.Fail("A category with this name already exists.");

        category.Name = name;
        category.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        category.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
        category.ImageAlt = string.IsNullOrWhiteSpace(request.ImageAlt) ? null : request.ImageAlt.Trim();
        category.AllowsToppings = request.AllowsToppings;
        category.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return ServiceResult<CategoryResponse>.Ok(CategoryResponse.From(category));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var category = await db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
            return ServiceResult<bool>.Fail("Category not found.");

        if (category.Products.Count > 0)
            return ServiceResult<bool>.Fail(
                $"Cannot delete \"{category.Name}\": {category.Products.Count} product(s) still use it. Move or delete them first.");

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
