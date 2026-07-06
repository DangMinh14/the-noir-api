using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

// Reads are public (the landing page uses them); writes are admin only.
[ApiController]
[Route("api/cities")]
[Authorize(Roles = UserRoles.Admin)]
public class CitiesController(ICityService cities) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<City>>> GetAll() =>
        await cities.GetAllAsync();

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<City>> GetById(int id)
    {
        var city = await cities.GetByIdAsync(id);
        return city is null ? NotFound() : city;
    }

    [HttpPost]
    public async Task<ActionResult<City>> Create(CityRequest request)
    {
        var city = await cities.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = city.Id }, city);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<City>> Update(int id, CityRequest request)
    {
        var city = await cities.UpdateAsync(id, request);
        return city is null ? NotFound() : city;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await cities.DeleteAsync(id) ? NoContent() : NotFound();
}
