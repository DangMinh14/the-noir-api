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
    ITokenService tokens,
    IEmailService emailService) : IAuthService
{
    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(u => u.Email == email))
            return ServiceResult<AuthResponse>.Fail("This email is already registered.");

        var now = DateTime.UtcNow;
        var user = new User
        {
            Email = email,
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = "",
            Role = UserRoles.User,
            CreatedAt = now,
            UpdatedAt = now,
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
        user.UpdatedAt = DateTime.UtcNow;
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
        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return ServiceResult<UserResponse>.Ok(UserResponse.From(user));
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Same generic message whether or not the account exists, so a
        // caller cannot use this endpoint to probe which emails are registered.
        const string genericMessage = "If that email has an account, a reset code has been sent to it.";
        if (user is null)
            return new ForgotPasswordResponse(genericMessage);

        user.ResetToken = RandomNumberGenerator.GetHexString(32);
        user.ResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(30);
        await db.SaveChangesAsync();

        await emailService.SendAsync(
            user.Email,
            "Reset your Thé Noir password",
            $"""
            <p>Someone asked to reset the password for this account.</p>
            <p>Your reset code (valid for 30 minutes):</p>
            <p style="font-size:20px;font-weight:600;letter-spacing:2px">{user.ResetToken}</p>
            <p>If this wasn't you, you can ignore this email.</p>
            """);

        return new ForgotPasswordResponse(genericMessage);
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
        user.UpdatedAt = DateTime.UtcNow;
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
