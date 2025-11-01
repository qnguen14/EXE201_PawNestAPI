using PawNest.Repository.Data.Metadata;
using PawNest.Repository.Data.Responses.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.User;

namespace PawNest.API.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    } 
    
    [HttpGet(ApiEndpointConstants.User.GetAllUsersEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<User>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")] // Admin-only access for user management
    public async Task<ActionResult<IEnumerable<CreateUserResponse>>> GetUsers()
    {
        // Service returns all users with basic profile information
        // Includes role information and account status
        var response = await _userService.GetAll();
    
        var apiResponse = new ApiResponse<IEnumerable<CreateUserResponse>>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Users retrieved successfully",
            IsSuccess = true,
            Data = response // Complete user list for administrative purposes
        };
        return Ok(apiResponse);
    }
    
    [HttpGet(ApiEndpointConstants.User.GetUserEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<CreateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")] // Admin-only access for user management
    public async Task<ActionResult<IEnumerable<CreateUserResponse>>> GetById(Guid id)
    {
        // Service returns all users with basic profile information
        // Includes role information and account status
        var response = await _userService.GetById(id);
    
        var apiResponse = new ApiResponse<CreateUserResponse>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = $"User {response.Name} retrieved successfully",
            IsSuccess = true,
            Data = response // Complete user list for administrative purposes
        };
        return Ok(apiResponse);
    }

    [HttpPut(ApiEndpointConstants.User.UpdateUserEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<GetUserReponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")] // Admin-only access for user management
    public async Task<ActionResult<CreateUserResponse>> UpdateUser([FromQuery] Guid userId ,[FromBody] UpdateUserRequest request)
    {
        var response = await _userService.Update(userId, request);

        var apiResponse = new ApiResponse<CreateUserResponse>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = $"User {response.Name} retrieved successfully",
            IsSuccess = true,
            Data = response // Complete user list for administrative purposes
        };
        return Ok(apiResponse);
    }

    [HttpPost(ApiEndpointConstants.User.CreateUserEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<CreateUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")] // Admin-only access for user management
    public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        var response = await _userService.Create(request);
        var apiResponse = new ApiResponse<CreateUserResponse>
        {
            StatusCode = StatusCodes.Status201Created,
            Message = $"User {response.Name} created successfully",
            IsSuccess = true,
            Data = response // Complete user list for administrative purposes
        };
        return Ok(apiResponse);
    }

    [HttpDelete(ApiEndpointConstants.User.GetUsersByRoleEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin, Staff")] // Admin-only access for user management
    public async Task<ActionResult> GetUsersByRole([FromQuery] string roleName)
    {
        var response = await _userService.GetUsersByRole(roleName);
        var apiResponse = new ApiResponse<object>
        {
            StatusCode = StatusCodes.Status200OK,
            Message = $"User with role: {roleName} retrieved successfully",
            IsSuccess = true,
            Data = null
        };
        return Ok(apiResponse);
    }
}