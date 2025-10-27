using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Metadata;
using PawNest.DAL.Data.Responses.User;
using PawNest.DAL.Data.MetaDatas;

namespace PawNest.API.Controllers
{
    [ApiController]
    public class FreelancerController : ControllerBase
    {
        private readonly IFreelancerService _freelancerService;
        private readonly ILogger<UserController> _logger;

        public FreelancerController(IFreelancerService freelancerService, ILogger<UserController> logger)
        {
            _freelancerService = freelancerService;
            _logger = logger;
        }

        [HttpGet(ApiEndpointConstants.User.GetAllFreelancersEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<PagingResponse<GetFreelancerResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff, Customer, Freelancer")]
        public async Task<ActionResult<PagingResponse<GetFreelancerResponse>>> GetFreelancers([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _freelancerService.GetAllFreelancersAsync(page, size);

            var apiResponse = new ApiResponse<PagingResponse<GetFreelancerResponse>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Freelancers retrieved successfully",
                IsSuccess = true,
                Data = response
            };
            return Ok(apiResponse);
        }

        [HttpGet(ApiEndpointConstants.User.GetFreelancerByIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetFreelancerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff, Customer, Freelancer")]
        public async Task<ActionResult<GetFreelancerResponse>> GetById(Guid id)
        {
            var response = await _freelancerService.GetFreelancerByIdAsync(id);

            var apiResponse = new ApiResponse<GetFreelancerResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Freelancer {response?.Name} retrieved successfully",
                IsSuccess = true,
                Data = response
            };
            return Ok(apiResponse);
        }

        [HttpGet(ApiEndpointConstants.User.SearchFreelancersEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetFreelancerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Staff, Customer, Freelancer")]
        public async Task<ActionResult<IEnumerable<GetFreelancerResponse>>> SearchFreelancersById([FromBody] string address, string serviceName)
        {
            var response = await _freelancerService.SearchFreelancers(address, serviceName);

            var apiResponse = new ApiResponse<IEnumerable<GetFreelancerResponse>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Freelancers retrieved successfully",
                IsSuccess = true,
                Data = response
            };
            return Ok(apiResponse);
        }
    }
}