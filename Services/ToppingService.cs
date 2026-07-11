using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class ToppingService(AppDbContext db) : IToppingService
{
    public Task<List<Topping>> GetAllAsync() =>
        db.Toppings.AsNoTracking().OrderBy(t => t.Name).ToListAsync();

    public Task<Topping?> GetByIdAsync(int id) =>
        db.Toppings.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

    public async Task<PagedResult<Topping>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = db.Toppings.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t => EF.Functions.Like(t.Name, $"%{term}%"));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Topping>(items, totalCount, page, pageSize);
    }

    public async Task<Topping> CreateAsync(ToppingRequest request)
    {
        var now = DateTime.UtcNow;
        var topping = new Topping
        {
            Name = request.Name.Trim(),
            PriceVnd = request.PriceVnd,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Toppings.Add(topping);
        await db.SaveChangesAsync();
        return topping;
    }

    public async Task<Topping?> UpdateAsync(int id, ToppingRequest request)
    {
        var topping = await db.Toppings.FindAsync(id);
        if (topping is null) return null;

        topping.Name = request.Name.Trim();
        topping.PriceVnd = request.PriceVnd;
        topping.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return topping;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var topping = await db.Toppings.FindAsync(id);
        if (topping is null) return false;

        db.Toppings.Remove(topping);
        await db.SaveChangesAsync();
        return true;
    }
}
