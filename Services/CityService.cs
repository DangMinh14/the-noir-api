using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class CityService(AppDbContext db) : ICityService
{
    public Task<List<City>> GetAllAsync() =>
        db.Cities.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync();

    public Task<City?> GetByIdAsync(int id) =>
        db.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<City> CreateAsync(CityRequest request)
    {
        var now = DateTime.UtcNow;
        var city = new City
        {
            Name = request.Name,
            MaisonCount = request.MaisonCount,
            Kind = request.Kind,
            Address = request.Address,
            SortOrder = request.SortOrder,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.Cities.Add(city);
        await db.SaveChangesAsync();
        return city;
    }

    public async Task<City?> UpdateAsync(int id, CityRequest request)
    {
        var city = await db.Cities.FindAsync(id);
        if (city is null) return null;

        city.Name = request.Name;
        city.MaisonCount = request.MaisonCount;
        city.Kind = request.Kind;
        city.Address = request.Address;
        city.SortOrder = request.SortOrder;
        city.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return city;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var city = await db.Cities.FindAsync(id);
        if (city is null) return false;

        db.Cities.Remove(city);
        await db.SaveChangesAsync();
        return true;
    }
}
