using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

// Reads are public (the landing page uses them); writes are admin only.
[ApiController]
[Route("api/products")]
[Authorize(Roles = UserRoles.Admin)]
public class ProductsController(IProductService products) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll() =>
        await products.GetAllAsync();

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var product = await products.GetByIdAsync(id);
        return product is null ? NotFound() : product;
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(ProductRequest request)
    {
        var result = await products.CreateAsync(request);
        if (!result.Succeeded) return BadRequest(new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductResponse>> Update(int id, ProductRequest request)
    {
        var result = await products.UpdateAsync(id, request);
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await products.DeleteAsync(id) ? NoContent() : NotFound();
}
