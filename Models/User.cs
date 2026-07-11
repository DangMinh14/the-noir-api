namespace TheNoir.Api.Models;

public static class UserRoles
{
    public const string User = "User";
    public const string Admin = "Admin";

    // Employee role: can manage orders (the header notifications modal) but
    // has no access to /admin (products, categories, users, etc).
    public const string Staff = "Staff";

    public static readonly string[] All = [User, Admin, Staff];

    // For [Authorize(Roles = ...)] on endpoints both Admin and Staff may use.
    public const string AdminOrStaff = $"{Admin},{Staff}";
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
