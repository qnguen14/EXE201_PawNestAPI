using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Responses.Image;
using PawNest.Repository.Mappers;
using PawNest.Repository.Repositories.Interfaces;
using PawNest.Services.Services.Interfaces;

namespace PawNest.Services.Services.Implements;

public class ImageService : BaseService<ImageService>, IImageService
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapperlyMapper _imageMapper;

    public ImageService(
        IUnitOfWork<PawNestDbContext> unitOfWork,
        ILogger<ImageService> logger,
        IHttpContextAccessor httpContextAccessor,
        IMapperlyMapper mapper,
        ICloudinaryService cloudinaryService)
        : base(unitOfWork, logger, httpContextAccessor, mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _imageMapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ImageResponse> UploadImageAsync(Stream imageStream, string fileName)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Upload to Cloudinary
                var uploadResult = await _cloudinaryService.UploadImageAsync(imageStream, fileName);

                // Create image entity
                var image = new Image
                {
                    Id = Guid.NewGuid(),
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url,
                    FileName = fileName,
                    Format = uploadResult.Format,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Size = uploadResult.Size,
                    UploadedAt = DateTime.UtcNow
                };

                // Save to database
                await _unitOfWork.GetRepository<Image>().InsertAsync(image);

                // Map to response using Mapperly
                return _imageMapper.MapToImageResponse(image);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while uploading image: " + ex.Message);
            throw;
        }
    }

    public async Task<ImageResponse> UploadBase64ImageAsync(string base64Image, string fileName)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Upload to Cloudinary
                var uploadResult = await _cloudinaryService.UploadImageAsync(base64Image, fileName);

                // Create image entity
                var image = new Image
                {
                    Id = Guid.NewGuid(),
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.Url,
                    FileName = fileName,
                    Format = uploadResult.Format,
                    Width = uploadResult.Width,
                    Height = uploadResult.Height,
                    Size = uploadResult.Size,
                    UploadedAt = DateTime.UtcNow
                };

                // Save to database
                await _unitOfWork.GetRepository<Image>().InsertAsync(image);

                // Map to response using Mapperly
                return _imageMapper.MapToImageResponse(image);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while uploading base64 image: " + ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteImageAsync(Guid imageId)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Get image from database
                var image = await _unitOfWork.GetRepository<Image>()
                    .FirstOrDefaultAsync(predicate: i => i.Id == imageId);

                if (image == null)
                    throw new NotFoundException($"Image with ID {imageId} not found.");

                // Delete from Cloudinary first
                var cloudinaryDeleted = await _cloudinaryService.DeleteImageAsync(image.PublicId);
                if (!cloudinaryDeleted)
                    throw new BadRequestException("Failed to delete image from Cloudinary.");

                // Delete from database
                _unitOfWork.GetRepository<Image>().DeleteAsync(image);
                return true;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting image: " + ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteImageByPublicIdAsync(string publicId)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Get image from database
                var image = await _unitOfWork.GetRepository<Image>()
                    .FirstOrDefaultAsync(predicate: i => i.PublicId == publicId);

                if (image == null)
                    throw new NotFoundException($"Image with public ID {publicId} not found.");

                // Delete from Cloudinary first
                var cloudinaryDeleted = await _cloudinaryService.DeleteImageAsync(publicId);
                if (!cloudinaryDeleted)
                    throw new BadRequestException("Failed to delete image from Cloudinary.");

                // Delete from database
                _unitOfWork.GetRepository<Image>().DeleteAsync(image);
                return true;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while deleting image by public ID: " + ex.Message);
            throw;
        }
    }

    public async Task<ImageResponse> GetImageByIdAsync(Guid imageId)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var image = await _unitOfWork.GetRepository<Image>()
                    .FirstOrDefaultAsync(
                        predicate: i => i.Id == imageId,
                        orderBy: null,
                        include: null
                    );

                if (image == null)
                    throw new NotFoundException($"Image with ID {imageId} not found.");

                return _imageMapper.MapToImageResponse(image);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving image: " + ex.Message);
            throw;
        }
    }

    public async Task<ImageResponse> GetImageByPublicIdAsync(string publicId)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var image = await _unitOfWork.GetRepository<Image>()
                    .FirstOrDefaultAsync(
                        predicate: i => i.PublicId == publicId,
                        orderBy: null,
                        include: null
                    );

                if (image == null)
                    throw new NotFoundException($"Image with public ID {publicId} not found.");

                return _imageMapper.MapToImageResponse(image);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving image by public ID: " + ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<ImageResponse>> GetAllImagesAsync()
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var images = await _unitOfWork.GetRepository<Image>()
                    .GetListAsync(
                        predicate: null,
                        orderBy: i => i.OrderByDescending(x => x.UploadedAt),
                        include: null
                    );

                return images.Select(image => _imageMapper.MapToImageResponse(image)).ToList();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving images: " + ex.Message);
            throw;
        }
    }
}
