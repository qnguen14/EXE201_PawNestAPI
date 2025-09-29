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
    }
}