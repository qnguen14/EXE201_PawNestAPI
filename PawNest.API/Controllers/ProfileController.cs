using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Repository.Data.Metadata;
using PawNest.Repository.Data.Requests.Profile;
using PawNest.Repository.Data.Responses.Profile;
using PawNest.Repository.Data.Responses.User;
using PawNest.Services.Services.Interfaces;
using System.Security.Claims;

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


        /// <summary>
        /// Lấy thông tin hồ sơ của người dùng hiện tại
        /// </summary>
        [Authorize]
        [HttpGet(ApiEndpointConstants.Profile.GetMyProfileEndpoint)]
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
        /// <summary>
        /// Lấy thông tin hồ sơ freelancer của người dùng hiện tại
        /// </summary>
        [Authorize]
        [HttpGet(ApiEndpointConstants.Profile.GetFreelancerProfileEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetFreelancerProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyFreelancerProfile()
        {
            try
            {
                var profile = await _profileService.GetFreelancerProfileAsync();
                var apiResponse = new ApiResponse<GetFreelancerProfile>
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
        /// <summary>
        /// Cập nhật thông tin profile của khách hàng hiện tại
        /// </summary>
        /// <param name="request">Thông tin cập nhật</param>
        [HttpPut(ApiEndpointConstants.Profile.UpdateMyProfileEndpoint)]
        [ProducesResponseType(typeof(GetUserProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateUserProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var profile = await _profileService.UpdateUserProfileAsync(request);
                return Ok(profile);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật thông tin profile của freelancer hiện tại
        /// </summary>
        /// <param name="request">Thông tin cập nhật</param>
        [HttpPut(ApiEndpointConstants.Profile.UpdateFreelancerProfileEndpoint)]
        [ProducesResponseType(typeof(GetFreelancerProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateFreelancerProfile([FromBody] UpdateFreelancerProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var profile = await _profileService.UpdateFreelancerProfileAsync(request);
                return Ok(profile);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
