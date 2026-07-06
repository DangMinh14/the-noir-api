using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = UserRoles.Admin)]
public class UsersController(IUserService users) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll() =>
        await users.GetAllAsync();

    [HttpPut("{id:int}/role")]
    public async Task<ActionResult<UserResponse>> UpdateRole(int id, UpdateRoleRequest request)
    {
        var result = await users.UpdateRoleAsync(id, request.Role, CurrentUserId());
        return result.Succeeded ? result.Value! : BadRequest(new { error = result.Error });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await users.DeleteAsync(id, CurrentUserId());
        return result.Succeeded ? NoContent() : BadRequest(new { error = result.Error });
    }

    private int CurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
