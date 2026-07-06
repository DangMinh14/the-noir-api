namespace TheNoir.Api.Services;

public class UploadService(IWebHostEnvironment env) : IUploadService
{
    private const long MaxSizeBytes = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> AllowedTypes = new()
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp",
    };

    public async Task<ServiceResult<string>> SaveProductImageAsync(IFormFile file)
    {
        if (file.Length == 0)
            return ServiceResult<string>.Fail("File is empty.");

        if (file.Length > MaxSizeBytes)
            return ServiceResult<string>.Fail("File is larger than 5 MB.");

        if (!AllowedTypes.TryGetValue(file.ContentType, out var extension))
            return ServiceResult<string>.Fail("Only JPG, PNG or WEBP images are allowed.");

        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        var uploadsDir = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(uploadsDir);

        // Generated filename, never the client-supplied one, so nothing in
        // the request can influence the path written to disk.
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using (var stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        return ServiceResult<string>.Ok($"/uploads/{fileName}");
    }
}
