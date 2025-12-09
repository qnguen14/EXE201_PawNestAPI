using PawNest.Repository.Data.Responses.Image;

namespace PawNest.Services.Services.Interfaces;

public interface IImageService
{
    Task<ImageResponse> UploadImageAsync(Stream imageStream, string fileName);
    Task<ImageResponse> UploadBase64ImageAsync(string base64Image, string fileName);
    Task<bool> DeleteImageAsync(Guid imageId);
    Task<bool> DeleteImageByPublicIdAsync(string publicId);
    Task<ImageResponse?> GetImageByIdAsync(Guid imageId);
    Task<ImageResponse?> GetImageByPublicIdAsync(string publicId);
    Task<IEnumerable<ImageResponse>> GetAllImagesAsync();
    
    
}

