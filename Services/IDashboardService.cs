using TheNoir.Api.Dtos;

namespace TheNoir.Api.Services;

public interface IDashboardService
{
    Task<DashboardStatsResponse> GetStatsAsync();
}
