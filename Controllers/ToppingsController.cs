using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

// Reads are public (drink customization uses them); writes are admin only.
[ApiController]
[Route("api/toppings")]
[Authorize(Roles = UserRoles.Admin)]
public class ToppingsController(IToppingService toppings) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<Topping>>> GetAll() =>
        await toppings.GetAllAsync();

    [AllowAnonymous]
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<Topping>>> Search(
        [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        await toppings.GetPagedAsync(search, page, pageSize);

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Topping>> GetById(int id)
    {
        var topping = await toppings.GetByIdAsync(id);
        return topping is null ? NotFound() : topping;
    }

    [HttpPost]
    public async Task<ActionResult<Topping>> Create(ToppingRequest request)
    {
        var topping = await toppings.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = topping.Id }, topping);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Topping>> Update(int id, ToppingRequest request)
    {
        var topping = await toppings.UpdateAsync(id, request);
        return topping is null ? NotFound() : topping;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await toppings.DeleteAsync(id) ? NoContent() : NotFound();
}
