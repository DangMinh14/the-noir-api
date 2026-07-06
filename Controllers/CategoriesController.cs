using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

// Reads are public (the product filter/dropdown needs them); writes are admin only.
[ApiController]
[Route("api/categories")]
[Authorize(Roles = UserRoles.Admin)]
public class CategoriesController(ICategoryService categories) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll() =>
        await categories.GetAllAsync();

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request)
    {
        var result = await categories.CreateAsync(request);
        return result.Succeeded ? result.Value! : Conflict(new { error = result.Error });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> Update(int id, CategoryRequest request)
    {
        var result = await categories.UpdateAsync(id, request);
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await categories.DeleteAsync(id);
        return result.Succeeded ? NoContent() : BadRequest(new { error = result.Error });
    }
}
