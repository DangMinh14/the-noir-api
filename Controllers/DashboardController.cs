using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = UserRoles.Admin)]
public class DashboardController(IDashboardService dashboard) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsResponse>> GetStats() =>
        await dashboard.GetStatsAsync();
}
