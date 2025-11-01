using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Metadata;
using PawNest.Repository.Data.Requests.Service;
using PawNest.Repository.Data.Responses.Service;

namespace PawNest.API.Controllers
{
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly IServiceService _serviceService;

        public ServiceController(ILogger<ServiceController> logger, IServiceService serviceService)
        {
            _logger = logger;
            _serviceService = serviceService;
        }

        [HttpGet(ApiEndpointConstants.Service.GetAllServicesEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetServiceResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Freelancer")] // Only Admins and Freelancers can create services
        public async Task<ActionResult<IEnumerable<GetServiceResponse>>> GetAllServices()
        {
            var services = await _serviceService.GetAllServicesAsync();
            var apiResponse = new ApiResponse<IEnumerable<GetServiceResponse>>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Services retrieved successfully",
                IsSuccess = true,
                Data = services
            };
            return Ok(apiResponse);
        }

        [HttpGet(ApiEndpointConstants.Service.GetServiceByIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetServiceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Freelancer")] // Only Admins and Freelancers can create services
        public async Task<ActionResult<GetServiceResponse>> GetServiceById(Guid id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null)
            {
                var notFoundResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Service not found",
                    IsSuccess = false,
                    Data = null
                };
                return NotFound(notFoundResponse);
            }
            var apiResponse = new ApiResponse<GetServiceResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Service retrieved successfully",
                IsSuccess = true,
                Data = service
            };
            return Ok(apiResponse);
        }

        [HttpPost(ApiEndpointConstants.Service.CreateServiceEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetServiceResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Freelancer")] // Only Admins and Freelancers can create services
        public async Task<ActionResult<GetServiceResponse>> CreateService([FromBody] CreateServiceRequest request)
        {
            var createdService = await _serviceService.CreateServiceAsync(request);
            var apiResponse = new ApiResponse<GetServiceResponse>
            {
                StatusCode = StatusCodes.Status201Created,
                Message = "Service created successfully",
                IsSuccess = true,
                Data = createdService
            };
            return Ok(apiResponse);
        }

        [HttpPut(ApiEndpointConstants.Service.UpdateServiceEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<GetServiceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Freelancer")] // Only Admins and Freelancers can update services
        public async Task<ActionResult<GetServiceResponse>> UpdateService(Guid id, [FromBody] UpdateServiceRequest request)
        {
            var updatedService = await _serviceService.UpdateServiceAsync(id, request);
            var apiResponse = new ApiResponse<GetServiceResponse>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Service updated successfully",
                IsSuccess = true,
                Data = updatedService
            };
            return Ok(apiResponse);
        }

        [HttpDelete(ApiEndpointConstants.Service.DeleteServiceEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin, Freelancer")] // Only Admins and Freelancers can delete services
        public async Task<ActionResult<bool>> DeleteService(Guid id)
        {
            var result = await _serviceService.DeleteServiceAsync(id);
            var apiResponse = new ApiResponse<bool>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Service deleted successfully",
                IsSuccess = true,
                Data = result
            };
            return Ok(apiResponse);
        }
    }
}
