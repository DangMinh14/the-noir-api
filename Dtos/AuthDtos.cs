using System.ComponentModel.DataAnnotations;
using TheNoir.Api.Models;

namespace TheNoir.Api.Dtos;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(200)]
    public required string Email { get; set; }

    [Required, MaxLength(100)]
    public required string DisplayName { get; set; }

    [Required, MinLength(8), MaxLength(100)]
    public required string Password { get; set; }
}

public class LoginRequest
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}

public class UpdateProfileRequest
{
    [Required, MaxLength(100)]
    public required string DisplayName { get; set; }
}

public class ChangePasswordRequest
{
    [Required]
    public required string CurrentPassword { get; set; }

    [Required, MinLength(8), MaxLength(100)]
    public required string NewPassword { get; set; }
}

public class ForgotPasswordRequest
{
    [Required, EmailAddress]
    public required string Email { get; set; }
}

public class ResetPasswordRequest
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string ResetToken { get; set; }

    [Required, MinLength(8), MaxLength(100)]
    public required string NewPassword { get; set; }
}

public class UpdateRoleRequest
{
    [Required]
    public required string Role { get; set; }
}

public record UserResponse(
    int Id,
    string Email,
    string DisplayName,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static UserResponse From(User user) =>
        new(user.Id, user.Email, user.DisplayName, user.Role, user.CreatedAt, user.UpdatedAt);
}

public record AuthResponse(string Token, DateTime ExpiresAt, UserResponse User);

// Dev-only shape: the reset token is returned in the response until real
// email delivery exists. Remove ResetToken from here once emails are sent.
public record ForgotPasswordResponse(string Message, string? ResetToken);
