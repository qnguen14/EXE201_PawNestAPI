// ============================================================================
// AUTHENTICATION CONTROLLER
// ============================================================================
// This controller handles all authentication-related operations for the Everwell system
// It manages user login, registration, password changes, and token management
// 
// FLOW EXPLANATION:
// 1. User submits login credentials (email/password)
// 2. AuthService validates credentials against database
// 3. If valid, JWT token is generated with user claims (ID, role, etc.)
// 4. Token is returned to client for subsequent API calls
// 5. Client includes token in Authorization header for protected endpoints
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Metadata;
using PawNest.DAL.Data.Requests.Auth;
using PawNest.DAL.Data.Responses.User;
using System.Linq;

namespace Everwell.API.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// USER LOGIN ENDPOINT
        /// ==================
        /// Authenticates user credentials and returns JWT token for API access
        /// 
        /// AUTHENTICATION FLOW:
        /// 1. Receive login request with email and password
        /// 2. AuthService validates credentials against database
        /// 3. If user exists and password matches (BCrypt verification):
        ///    - Generate JWT token with user claims (ID, role, email)
        ///    - Return token with user profile information
        /// 4. If credentials invalid, return appropriate error
        /// 
        /// SECURITY FEATURES:
        /// - Password hashing with BCrypt
        /// - JWT token with expiration
        /// - Role-based claims for authorization
        /// - Account status validation (active/inactive)
        /// </summary>
        [HttpPost(ApiEndpointConstants.Auth.LoginEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Call business logic layer to handle authentication
            var response = await _authService.Login(request);

            // Handle case where user account doesn't exist
            if (response == null)
            {
                return NotFound(new ApiResponse<LoginResponse>
                {
                    Message = "Tài khoản không tồn tại.", // "Account does not exist"
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            // Handle case where password is incorrect
            if (response.IsUnauthorized)
            {
                return Unauthorized(new ApiResponse<LoginResponse>
                {
                    Message = "Mật khẩu không đúng.", // "Password is incorrect"
                    IsSuccess = false,
                    StatusCode = StatusCodes.Status401Unauthorized
                });
            }

            // Successful login - return JWT token and user information
            var apiResponse = new ApiResponse<LoginResponse>
            {
                Message = "Đăng nhập thành công.", // "Login successful"
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = response // Contains JWT token, user profile, role information
            };

            return Ok(apiResponse);
        }

        [HttpPost(ApiEndpointConstants.Auth.RegisterEndpoint)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Basic null check for request object
            if (request == null)
            {
                return BadRequest("Invalid registration request.");
            }

            // Validate model state (data annotations validation)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // AuthService handles:
                // 1. Email uniqueness validation
                // 2. Password hashing
                // 3. User entity creation
                // 4. Role assignment
                // 5. Database persistence
                var response = await _authService.Register(request);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    // Return validation errors (e.g., email already exists)
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Success = false, Message = "An error occurred during registration." });
            }
        }

        [HttpPost(ApiEndpointConstants.Auth.LogoutEndpoint)]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Extract JWT token from Authorization header ("Bearer <token>")
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    return BadRequest("Authorization header is missing.");
                }

                // AuthService handles:
                // 1. Token extraction from "Bearer <token>" format
                // 2. Adding token to BlacklistedToken table
                // 3. Database persistence
                var response = await _authService.Logout(authHeader);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Success = false, Message = "An error occurred during logout." });
            }
        }
    }
}