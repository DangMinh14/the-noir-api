using TheNoir.Api.Dtos;

namespace TheNoir.Api.Services;

// Admin-only user management.
public interface IUserService
{
    Task<PagedResult<UserResponse>> GetAllAsync(string? search, int page, int pageSize);
    Task<ServiceResult<UserResponse>> UpdateRoleAsync(int id, string role, int actingUserId);
    Task<ServiceResult<bool>> DeleteAsync(int id, int actingUserId);
}
