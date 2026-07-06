using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public interface ICityService
{
    Task<List<City>> GetAllAsync();
    Task<PagedResult<City>> GetPagedAsync(string? search, int page, int pageSize);
    Task<City?> GetByIdAsync(int id);
    Task<City> CreateAsync(CityRequest request);
    Task<City?> UpdateAsync(int id, CityRequest request);
    Task<bool> DeleteAsync(int id);
}
