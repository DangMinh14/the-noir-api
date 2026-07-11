using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheNoir.Api.Dtos;
using TheNoir.Api.Models;
using TheNoir.Api.Services;

namespace TheNoir.Api.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize(Roles = UserRoles.Admin)]
public class UploadsController(IUploadService uploads) : ControllerBase
{
    [HttpPost("image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<UploadResponse>> UploadImage(IFormFile file)
    {
        var result = await uploads.SaveImageAsync(file);
        return result.Succeeded
            ? new UploadResponse(result.Value!)
            : BadRequest(new { error = result.Error });
    }
}
