using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await auth.RegisterAsync(request);
        return result.Succeeded ? result.Value! : Conflict(new { error = result.Error });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await auth.LoginAsync(request);
        return result.Succeeded ? result.Value! : Unauthorized(new { error = result.Error });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var user = await auth.GetByIdAsync(CurrentUserId());
        return user is null ? Unauthorized() : user;
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<UserResponse>> UpdateProfile(UpdateProfileRequest request)
    {
        var result = await auth.UpdateProfileAsync(CurrentUserId(), request);
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<UserResponse>> ChangePassword(ChangePasswordRequest request)
    {
        var result = await auth.ChangePasswordAsync(CurrentUserId(), request);
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordRequest request) =>
        await auth.ForgotPasswordAsync(request);

    [HttpPost("reset-password")]
    public async Task<ActionResult<UserResponse>> ResetPassword(ResetPasswordRequest request)
    {
        var result = await auth.ResetPasswordAsync(request);
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    private int CurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
