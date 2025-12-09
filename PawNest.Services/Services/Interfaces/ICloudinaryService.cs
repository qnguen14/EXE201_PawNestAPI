using PawNest.Repository.Data.Responses.Image;
using PawNest.Repository.Data.Responses.Video;

namespace PawNest.Services.Services.Interfaces;

public interface ICloudinaryService
{
    // Existing image methods
    Task<ImageUploadResult> UploadImageAsync(Stream imageStream, string fileName);
    Task<ImageUploadResult> UploadImageAsync(string base64Image, string fileName);
    Task<bool> DeleteImageAsync(string publicId);
    Task<string> GetImageUrlAsync(string publicId, int? width = null, int? height = null);
    
    // New video methods
    Task<VideoUploadResult> UploadVideoAsync(Stream videoStream, string fileName);
    Task<VideoUploadResult> UploadVideoAsync(string base64Video, string fileName);
    Task<bool> DeleteVideoAsync(string publicId);
    Task<string> GetVideoUrlAsync(string publicId, int? width = null, int? height = null);
}
