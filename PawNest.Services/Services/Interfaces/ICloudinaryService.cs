using PawNest.Repository.Data.Responses.Image;

namespace PawNest.Services.Services.Interfaces;

public interface ICloudinaryService
{
    Task<ImageUploadResult> UploadImageAsync(Stream imageStream, string fileName);
    Task<ImageUploadResult> UploadImageAsync(string base64Image, string fileName);
    Task<bool> DeleteImageAsync(string publicId);
    Task<string> GetImageUrlAsync(string publicId, int? width = null, int? height = null);
}