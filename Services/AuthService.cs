using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheNoir.Api.Data;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;

namespace TheNoir.Api.Services;

public class AuthService(
    AppDbContext db,
    IPasswordHasher<User> hasher,
    ITokenService tokens) : IAuthService
{
    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(u => u.Email == email))
            return ServiceResult<AuthResponse>.Fail("This email is already registered.");

        var user = new User
        {
            Email = email,
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = "",
            Role = UserRoles.User,
            CreatedAt = DateTime.UtcNow,
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return ServiceResult<AuthResponse>.Ok(BuildAuthResponse(user));
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Same message for unknown email and wrong password, so callers
        // cannot probe which emails have an account.
        if (user is null ||
            hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
        {
            return ServiceResult<AuthResponse>.Fail("Email or password is incorrect.");
        }

        return ServiceResult<AuthResponse>.Ok(BuildAuthResponse(user));
    }

    public async Task<UserResponse?> GetByIdAsync(int userId)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        return user is null ? null : UserResponse.From(user);
    }

    public async Task<ServiceResult<UserResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await db.Users.FindAsync(userId);
        if (user is null)
            return ServiceResult<UserResponse>.Fail("Account no longer exists.");

        user.DisplayName = request.DisplayName.Trim();
        await db.SaveChangesAsync();
        return ServiceResult<UserResponse>.Ok(UserResponse.From(user));
    }

    public async Task<ServiceResult<UserResponse>> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await db.Users.FindAsync(userId);
        if (user is null)
            return ServiceResult<UserResponse>.Fail("Account no longer exists.");

        if (hasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword)
                == PasswordVerificationResult.Failed)
        {
            return ServiceResult<UserResponse>.Fail("Current password is incorrect.");
        }

        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
        await db.SaveChangesAsync();
        return ServiceResult<UserResponse>.Ok(UserResponse.From(user));
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Dev-only behaviour: the token is handed back in the response.
        // Once email sending exists, always return the generic message
        // and deliver the token by email instead.
        if (user is null)
            return new ForgotPasswordResponse("If that email has an account, a reset code has been issued.", null);

        user.ResetToken = RandomNumberGenerator.GetHexString(32);
        user.ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(30);
        await db.SaveChangesAsync();

        return new ForgotPasswordResponse("Reset code issued. It expires in 30 minutes.", user.ResetToken);
    }

    public async Task<ServiceResult<UserResponse>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null ||
            user.ResetToken is null ||
            user.ResetTokenExpiresAt < DateTime.UtcNow ||
            !CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(user.ResetToken),
                TryFromHex(request.ResetToken)))
        {
            return ServiceResult<UserResponse>.Fail("Reset code is invalid or has expired.");
        }

        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiresAt = null;
        await db.SaveChangesAsync();

        return ServiceResult<UserResponse>.Ok(UserResponse.From(user));
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAt) = tokens.CreateToken(user);
        return new AuthResponse(token, expiresAt, UserResponse.From(user));
    }

    private static byte[] TryFromHex(string value)
    {
        try { return Convert.FromHexString(value); }
        catch (FormatException) { return []; }
    }
}
