using TheNoir.Api.Dtos;

namespace TheNoir.Api.Services;

public interface IOrderService
{
    Task<ServiceResult<OrderResponse>> CreateAsync(int userId, CreateOrderRequest request);
    Task<List<OrderResponse>> GetMineAsync(int userId);
    Task<OrderResponse?> GetByIdAsync(int id, int requestingUserId, bool isAdmin);
    Task<List<OrderResponse>> GetAllAsync();
    Task<ServiceResult<OrderResponse>> UpdateStatusAsync(int id, string status);
}
