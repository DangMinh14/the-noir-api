using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(ProductRequest request);
    Task<Product?> UpdateAsync(int id, ProductRequest request);
    Task<bool> DeleteAsync(int id);
}
