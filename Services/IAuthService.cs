using TheNoir.Api.Dtos;

namespace TheNoir.Api.Services;

public interface IAuthService
{
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<UserResponse?> GetByIdAsync(int userId);
    Task<ServiceResult<UserResponse>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<ServiceResult<UserResponse>> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ServiceResult<UserResponse>> ResetPasswordAsync(ResetPasswordRequest request);
}
