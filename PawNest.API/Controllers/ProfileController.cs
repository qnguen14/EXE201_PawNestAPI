using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.Services.Services.Interfaces;
using System.Security.Claims;
using PawNest.API.Constants;
using PawNest.Repository.Data.Metadata;
using PawNest.Repository.Data.Responses.Profile;
using PawNest.Repository.Data.Responses.User;

namespace PawNest.API.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileController(IProfileService profileService, IHttpContextAccessor httpContextAccessor)
        {
            _profileService = profileService;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ Cách 2: Lấy profile của chính user (sau khi đăng nhập)
        // Cần token JWT, trong đó có userId
        [Authorize]
        [HttpGet(ApiEndpointConstants.User.GetMyProfileEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetUserProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var profile = await _profileService.GetProfileAsync();
                var apiResponse = new ApiResponse<GetUserProfile>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Profile retrieved successfully",
                    IsSuccess = true,
                    Data = profile //
                };
                return Ok(apiResponse);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

    }
}
