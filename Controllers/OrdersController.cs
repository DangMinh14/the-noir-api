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
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request)
    {
        var result = await orders.CreateAsync(CurrentUserId(), request);
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

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAll() =>
        await orders.GetAllAsync();

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(int id, UpdateOrderStatusRequest request)
    {
        var result = await orders.UpdateStatusAsync(id, request.Status);
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    private int CurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
