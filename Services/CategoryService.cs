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

    public async Task<ServiceResult<CategoryResponse>> CreateAsync(CategoryRequest request)
    {
        var name = request.Name.Trim();
        if (await db.Categories.AnyAsync(c => c.Name == name))
            return ServiceResult<CategoryResponse>.Fail("A category with this name already exists.");

        var now = DateTime.UtcNow;
        var category = new Category { Name = name, CreatedAt = now, UpdatedAt = now };

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
