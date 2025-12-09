using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Services.Services.Interfaces;

namespace PawNest.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route(ApiEndpointConstants.Cloudinary.CloudinaryEndpoint)]
    public class CloudinaryController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ICloudinaryService _cloudinaryService;

        public CloudinaryController(IImageService imageService, ICloudinaryService cloudinaryService)
        {
            _imageService = imageService;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Uploads an image file to Cloudinary and saves to database.
        /// </summary>
        [HttpPost(ApiEndpointConstants.Cloudinary.UploadImageEndpoint)]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _imageService.UploadImageAsync(stream, file.FileName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }

        /// <summary>
        /// Uploads a base64 encoded image to Cloudinary and saves to database.
        /// </summary>
        [HttpPost("upload-base64")]
        public async Task<IActionResult> UploadBase64Image([FromBody] Base64ImageRequest request)
        {
            if (string.IsNullOrEmpty(request.Base64Image))
                return BadRequest("No image data provided");

            try
            {
                var result = await _imageService.UploadBase64ImageAsync(request.Base64Image, request.FileName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an image from Cloudinary and database by its public ID.
        /// </summary>
        [HttpDelete(ApiEndpointConstants.Cloudinary.DeleteImageEndpoint)]
        public async Task<IActionResult> DeleteImage(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return BadRequest("Public ID is required");

            try
            {
                var result = await _imageService.DeleteImageByPublicIdAsync(publicId);
                return result 
                    ? Ok(new { message = "Image deleted successfully" }) 
                    : NotFound("Image not found or already deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting image: {ex.Message}");
            }
        }

        /// <summary>
        /// Get image by ID from database.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetImageById(Guid id)
        {
            try
            {
                var image = await _imageService.GetImageByIdAsync(id);
                return Ok(image);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving image: {ex.Message}");
            }
        }

        /// <summary>
        /// Get image by public ID from database.
        /// </summary>
        [HttpGet("public")]
        public async Task<IActionResult> GetImageByPublicId([FromQuery] string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return BadRequest("Public ID is required");

            try
            {
                var image = await _imageService.GetImageByPublicIdAsync(publicId);
                return Ok(image);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving image: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all images from database.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            try
            {
                var images = await _imageService.GetAllImagesAsync();
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving images: {ex.Message}");
            }
        }

        /// <summary>
        /// Get transformed image URL from Cloudinary (utility endpoint).
        /// </summary>
        [HttpGet(ApiEndpointConstants.Cloudinary.GetImageUrlEndpoint)]
        public async Task<IActionResult> GetImageUrl(string publicId, [FromQuery] int? width, [FromQuery] int? height)
        {
            if (string.IsNullOrEmpty(publicId))
                return BadRequest("Public ID is required");

            try
            {
                var url = await _cloudinaryService.GetImageUrlAsync(publicId, width, height);
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting image URL: {ex.Message}");
            }
        }
    }

    public class Base64ImageRequest
    {
        public string Base64Image { get; set; } = string.Empty;
        public string FileName { get; set; } = "image.png";
    }
}
