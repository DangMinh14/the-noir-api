using TheNoir.Api.Dtos;

namespace TheNoir.Api.Services;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync();
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<ServiceResult<ProductResponse>> CreateAsync(ProductRequest request);
    Task<ServiceResult<ProductResponse>> UpdateAsync(int id, ProductRequest request);
    Task<bool> DeleteAsync(int id);
}
