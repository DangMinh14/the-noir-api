using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(IOrderService orders) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request)
    {
        var result = await orders.CreateAsync(CurrentUserIdOrNull(), request);
        if (!result.Succeeded) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet("mine")]
    public async Task<ActionResult<List<OrderResponse>>> GetMine() =>
        await orders.GetMineAsync(CurrentUserId());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetById(int id)
    {
        var order = await orders.GetByIdAsync(id, CurrentUserId(), User.IsInRole(UserRoles.Admin));
        return order is null ? NotFound() : order;
    }

    [Authorize(Roles = UserRoles.AdminOrStaff)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderResponse>>> GetAll(
        [FromQuery] string? search, [FromQuery] string? status,
        [FromQuery] int? cityId, [FromQuery] bool? autoCancelled,
        [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        await orders.GetAllAsync(search, status, cityId, autoCancelled, from, to, page, pageSize);

    [Authorize(Roles = UserRoles.AdminOrStaff)]
    [HttpGet("unseen-summary")]
    public async Task<ActionResult<UnseenOrdersSummary>> GetUnseenSummary([FromQuery] DateTime since) =>
        await orders.GetUnseenSummaryAsync(since);

    [Authorize(Roles = UserRoles.AdminOrStaff)]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(int id, UpdateOrderStatusRequest request)
    {
        var result = await orders.UpdateStatusAsync(id, request.Status);
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:int}/messages")]
    public async Task<ActionResult<List<OrderMessageResponse>>> GetMessages(int id)
    {
        var result = await orders.GetMessagesAsync(id, CurrentUserId(), IsAdminOrStaff());
        return result.Succeeded ? result.Value! : NotFound(new { error = result.Error });
    }

    [HttpPost("{id:int}/messages")]
    public async Task<ActionResult<OrderMessageResponse>> AddMessage(int id, CreateOrderMessageRequest request)
    {
        var result = await orders.AddMessageAsync(id, CurrentUserId(), IsAdminOrStaff(), request.Body);
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    private bool IsAdminOrStaff() =>
        User.IsInRole(UserRoles.Admin) || User.IsInRole(UserRoles.Staff);

    private int CurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private int? CurrentUserIdOrNull() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
