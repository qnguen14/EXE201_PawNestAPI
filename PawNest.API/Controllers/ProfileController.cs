using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PawNest.API.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        // ✅ Cách 1: Lấy theo userId (dễ test, dùng khi chưa có token)
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            try
            {
                var profile = await _profileService.GetProfileAsync(userId);
                return Ok(profile);
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

        // ✅ Cách 2: Lấy profile của chính user (sau khi đăng nhập)
        // Cần token JWT, trong đó có userId
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Invalid token." });

                var userId = Guid.Parse(userIdClaim);
                var profile = await _profileService.GetProfileAsync(userId);

                return Ok(profile);
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
