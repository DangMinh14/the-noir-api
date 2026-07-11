using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public interface IToppingService
{
    Task<List<Topping>> GetAllAsync();
    Task<PagedResult<Topping>> GetPagedAsync(string? search, int page, int pageSize);
    Task<Topping?> GetByIdAsync(int id);
    Task<Topping> CreateAsync(ToppingRequest request);
    Task<Topping?> UpdateAsync(int id, ToppingRequest request);
    Task<bool> DeleteAsync(int id);
}
