using PawNest.Repository.Data.Responses.Video;

namespace PawNest.Services.Services.Interfaces;

public interface IVideoService
{
    Task<VideoResponse> UploadVideoAsync(Stream videoStream, string fileName);
    Task<VideoResponse> UploadBase64VideoAsync(string base64Video, string fileName);
    Task<bool> DeleteVideoAsync(Guid videoId);
    Task<bool> DeleteVideoByPublicIdAsync(string publicId);
    Task<VideoResponse> GetVideoByIdAsync(Guid videoId);
    Task<VideoResponse> GetVideoByPublicIdAsync(string publicId);
    Task<IEnumerable<VideoResponse>> GetAllVideosAsync();
}