using TheNoir.Api.Dtos;

namespace TheNoir.Api.Services;

public interface IOrderService
{
    Task<ServiceResult<OrderResponse>> CreateAsync(int? userId, CreateOrderRequest request);
    Task<List<OrderResponse>> GetMineAsync(int userId);
    Task<OrderResponse?> GetByIdAsync(int id, int requestingUserId, bool isAdmin);
    Task<PagedResult<OrderResponse>> GetAllAsync(
        string? search, string? status, int? cityId, bool? autoCancelled,
        DateTime? from, DateTime? to, int page, int pageSize);
    Task<UnseenOrdersSummary> GetUnseenSummaryAsync(DateTime since);
    Task<ServiceResult<OrderResponse>> UpdateStatusAsync(int id, string status);
    Task<ServiceResult<List<OrderMessageResponse>>> GetMessagesAsync(int orderId, int requestingUserId, bool isAdminOrStaff);
    Task<ServiceResult<OrderMessageResponse>> AddMessageAsync(int orderId, int requestingUserId, bool isAdminOrStaff, string body);
}
