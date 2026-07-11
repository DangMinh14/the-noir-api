using TheNoir.Api.Dtos;

namespace TheNoir.Api.Services;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync();
    Task<PagedResult<CategoryResponse>> GetPagedAsync(string? search, int page, int pageSize);
    Task<CategoryResponse?> GetByIdAsync(int id);
    Task<ServiceResult<CategoryResponse>> CreateAsync(CategoryRequest request);
    Task<ServiceResult<CategoryResponse>> UpdateAsync(int id, CategoryRequest request);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}
