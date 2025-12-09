using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using PawNest.Services.Services.Interfaces;
using ImageUploadResult = PawNest.Repository.Data.Responses.Image.ImageUploadResult;
using VideoUploadResult = PawNest.Repository.Data.Responses.Video.VideoUploadResult;

namespace PawNest.Services.Services.Implements;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<ImageUploadResult> UploadImageAsync(Stream imageStream, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, imageStream),
            Folder = "pawnest",
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return new ImageUploadResult
        {
            PublicId = uploadResult.PublicId,
            Url = uploadResult.SecureUrl.ToString(),
            Format = uploadResult.Format,
            Width = uploadResult.Width,
            Height = uploadResult.Height,
            Size = uploadResult.Bytes
        };
    }

    public async Task<ImageUploadResult> UploadImageAsync(string base64Image, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, base64Image),
            Folder = "pawnest",
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return new ImageUploadResult
        {
            PublicId = uploadResult.PublicId,
            Url = uploadResult.SecureUrl.ToString(),
            Format = uploadResult.Format,
            Width = uploadResult.Width,
            Height = uploadResult.Height,
            Size = uploadResult.Bytes
        };
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }

    public Task<string> GetImageUrlAsync(string publicId, int? width = null, int? height = null)
    {
        var transformation = new Transformation();
        
        if (width.HasValue)
            transformation.Width(width.Value);
        
        if (height.HasValue)
            transformation.Height(height.Value);

        var url = _cloudinary.Api.UrlImgUp.Transform(transformation).BuildUrl(publicId);
        return Task.FromResult(url);
    }
    
    public async Task<VideoUploadResult> UploadVideoAsync(Stream videoStream, string fileName)
    {
        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(fileName, videoStream),
            Folder = "pawnest",
            PublicId = Path.GetFileNameWithoutExtension(fileName),
            Overwrite = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception($"Cloudinary video upload failed: {result.Error.Message}");

        return new VideoUploadResult
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString(),
            Format = result.Format,
            Duration = result.Duration,
            Width = result.Width,
            Height = result.Height,
            Size = result.Bytes
        };
    }

    public async Task<VideoUploadResult> UploadVideoAsync(string base64Video, string fileName)
    {
        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(fileName, $"data:video/mp4;base64,{base64Video}"),
            Folder = "pawnest",
            PublicId = Path.GetFileNameWithoutExtension(fileName),
            Overwrite = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception($"Cloudinary video upload failed: {result.Error.Message}");

        return new VideoUploadResult
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString(),
            Format = result.Format,
            Duration = result.Duration,
            Width = result.Width,
            Height = result.Height,
            Size = result.Bytes
        };
    }

    public async Task<bool> DeleteVideoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Video
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }

    
    public async Task<string> GetVideoUrlAsync(string publicId, int? width = null, int? height = null)
    {
        var transformation = new Transformation();
        
        if (width.HasValue)
            transformation = transformation.Width(width.Value);
        
        if (height.HasValue)
            transformation = transformation.Height(height.Value);
    
        return _cloudinary.Api.UrlVideoUp
            .Transform(transformation)
            .ResourceType("video")
            .BuildUrl(publicId);
    }
    
}
