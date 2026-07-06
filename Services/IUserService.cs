using TheNoir.Api.Dtos;

namespace TheNoir.Api.Services;

// Admin-only user management.
public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();
    Task<ServiceResult<UserResponse>> UpdateRoleAsync(int id, string role, int actingUserId);
    Task<ServiceResult<bool>> DeleteAsync(int id, int actingUserId);
}
