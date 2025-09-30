using PawNest.DAL.Data.Metadata;
using PawNest.DAL.Data.Responses.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Entities;

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
}