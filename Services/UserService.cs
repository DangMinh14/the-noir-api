using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class UserService(AppDbContext db) : IUserService
{
    public async Task<List<UserResponse>> GetAllAsync()
    {
        var users = await db.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync();
        return users.Select(UserResponse.From).ToList();
    }

    public async Task<ServiceResult<UserResponse>> UpdateRoleAsync(int id, string role, int actingUserId)
    {
        if (!UserRoles.All.Contains(role))
            return ServiceResult<UserResponse>.Fail($"Role must be one of: {string.Join(", ", UserRoles.All)}.");

        if (id == actingUserId)
            return ServiceResult<UserResponse>.Fail("You cannot change your own role.");

        var user = await db.Users.FindAsync(id);
        if (user is null)
            return ServiceResult<UserResponse>.Fail("User not found.");

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return ServiceResult<UserResponse>.Ok(UserResponse.From(user));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id, int actingUserId)
    {
        if (id == actingUserId)
            return ServiceResult<bool>.Fail("You cannot delete your own account from here.");

        var user = await db.Users.FindAsync(id);
        if (user is null)
            return ServiceResult<bool>.Fail("User not found.");

        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
