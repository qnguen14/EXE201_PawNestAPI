using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Responses.Video;
using PawNest.Repository.Mappers;
using PawNest.Repository.Repositories.Interfaces;
using PawNest.Services.Services.Interfaces;

namespace PawNest.Services.Services.Implements;

public class VideoService : BaseService<VideoService>, IVideoService
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapperlyMapper _videoMapper;

    public VideoService(
        IUnitOfWork<PawNestDbContext> unitOfWork,
        ILogger<VideoService> logger,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper,
        ICloudinaryService cloudinaryService)
        : base(unitOfWork, logger, httpContextAccessor, mapper)
    {
        _cloudinaryService = cloudinaryService;
        _videoMapper = mapper;
    }

    public async Task<VideoResponse> UploadVideoAsync(Stream videoStream, string fileName)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var uploadResult = await _cloudinaryService.UploadVideoAsync(videoStream, fileName);

                var video = new Video
                {
                    Id = Guid.NewGuid(),
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url,
                    FileName = fileName,
                    Format = uploadResult.Format,
                    Duration = uploadResult.Duration,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Size = uploadResult.Size,
                    UploadedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<Video>().InsertAsync(video);
                return _videoMapper.MapToVideoResponse(video);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error uploading video: " + ex.Message);
            throw;
        }
    }

    public async Task<VideoResponse> UploadBase64VideoAsync(string base64Video, string fileName)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var uploadResult = await _cloudinaryService.UploadVideoAsync(base64Video, fileName);

                var video = new Video
                {
                    Id = Guid.NewGuid(),
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url,
                    FileName = fileName,
                    Format = uploadResult.Format,
                    Duration = uploadResult.Duration,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Size = uploadResult.Size,
                    UploadedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<Video>().InsertAsync(video);
                return _videoMapper.MapToVideoResponse(video);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error uploading base64 video: " + ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteVideoAsync(Guid videoId)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var video = await _unitOfWork.GetRepository<Video>()
                    .FirstOrDefaultAsync(predicate: v => v.Id == videoId);

                if (video == null)
                    throw new NotFoundException($"Video with ID {videoId} not found.");

                var cloudinaryDeleted = await _cloudinaryService.DeleteVideoAsync(video.PublicId);
                if (!cloudinaryDeleted)
                    throw new BadRequestException("Failed to delete video from Cloudinary.");

                _unitOfWork.GetRepository<Video>().DeleteAsync(video);
                return true;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting video: " + ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteVideoByPublicIdAsync(string publicId)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var video = await _unitOfWork.GetRepository<Video>()
                    .FirstOrDefaultAsync(predicate: v => v.PublicId == publicId);

                if (video == null)
                    throw new NotFoundException($"Video with public ID {publicId} not found.");

                var cloudinaryDeleted = await _cloudinaryService.DeleteVideoAsync(publicId);
                if (!cloudinaryDeleted)
                    throw new BadRequestException("Failed to delete video from Cloudinary.");

                _unitOfWork.GetRepository<Video>().DeleteAsync(video);
                return true;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting video by public ID: " + ex.Message);
            throw;
        }
    }

    public async Task<VideoResponse> GetVideoByIdAsync(Guid videoId)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var video = await _unitOfWork.GetRepository<Video>()
                    .FirstOrDefaultAsync(predicate: v => v.Id == videoId);

                if (video == null)
                    throw new NotFoundException($"Video with ID {videoId} not found.");

                return _videoMapper.MapToVideoResponse(video);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving video: " + ex.Message);
            throw;
        }
    }

    public async Task<VideoResponse> GetVideoByPublicIdAsync(string publicId)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var video = await _unitOfWork.GetRepository<Video>()
                    .FirstOrDefaultAsync(predicate: v => v.PublicId == publicId);

                if (video == null)
                    throw new NotFoundException($"Video with public ID {publicId} not found.");

                return _videoMapper.MapToVideoResponse(video);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving video by public ID: " + ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<VideoResponse>> GetAllVideosAsync()
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var videos = await _unitOfWork.GetRepository<Video>()
                    .GetListAsync(
                        predicate: null,
                        orderBy: v => v.OrderByDescending(x => x.UploadedAt),
                        include: null
                    );

                return videos.Select(video => _videoMapper.MapToVideoResponse(video)).ToList();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error retrieving videos: " + ex.Message);
            throw;
        }
    }
}
