namespace TheNoir.Api.Models;

public static class UserRoles
{
    public const string User = "User";
    public const string Admin = "Admin";

    public static readonly string[] All = [User, Admin];
}

public class User
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = UserRoles.User;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Set by the forgot-password flow, cleared once used.
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiresAt { get; set; }
}
