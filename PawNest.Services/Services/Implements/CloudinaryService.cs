using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using PawNest.Services.Services.Interfaces;
using ImageUploadResult = PawNest.Repository.Data.Responses.Image.ImageUploadResult;

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
}
