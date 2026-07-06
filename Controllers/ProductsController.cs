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
    public async Task<ActionResult<List<Product>>> GetAll() =>
        await products.GetAllAsync();

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await products.GetByIdAsync(id);
        return product is null ? NotFound() : product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(ProductRequest request)
    {
        var product = await products.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> Update(int id, ProductRequest request)
    {
        var product = await products.UpdateAsync(id, request);
        return product is null ? NotFound() : product;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await products.DeleteAsync(id) ? NoContent() : NotFound();
}
