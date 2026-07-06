using Microsoft.AspNetCore.Http;

namespace TheNoir.Api.Services;

public interface IUploadService
{
    Task<ServiceResult<string>> SaveProductImageAsync(IFormFile file);
}
