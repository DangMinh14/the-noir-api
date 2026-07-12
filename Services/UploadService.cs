using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace TheNoir.Api.Services;

// Uploads to Cloudinary instead of the local disk: Render's free tier wipes
// its container filesystem on every redeploy/restart, which was silently
// deleting product/category photos. Cloudinary URLs are absolute and stored
// in the (persistent) Neon database, so images survive redeploys and are
// shared between local dev and production without any file copying.
public class UploadService(Cloudinary cloudinary) : IUploadService
{
    private const long MaxSizeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedTypes = new()
    {
        "image/jpeg", "image/png", "image/webp",
    };

    public async Task<ServiceResult<string>> SaveImageAsync(IFormFile file)
    {
        if (file.Length == 0)
            return ServiceResult<string>.Fail("File is empty.");

        if (file.Length > MaxSizeBytes)
            return ServiceResult<string>.Fail("File is larger than 5 MB.");

        if (!AllowedTypes.Contains(file.ContentType))
            return ServiceResult<string>.Fail("Only JPG, PNG or WEBP images are allowed.");

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "thenoir",
        };

        var result = await cloudinary.UploadAsync(uploadParams);
        if (result.Error is not null)
            return ServiceResult<string>.Fail(result.Error.Message);

        return ServiceResult<string>.Ok(result.SecureUrl.ToString());
    }
}
