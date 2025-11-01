using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Repository.Data.Metadata;
using PawNest.Repository.Data.Requests.Review;
using PawNest.Services.Services.Interfaces;

namespace PawNest.API.Controllers
{
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet(ApiEndpointConstants.Review.GetAllReviewsEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAll();
                var apiResponse = new ApiResponse<IEnumerable<object>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Reviews retrieved successfully",
                    IsSuccess = true,
                    Data = reviews
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet(ApiEndpointConstants.Review.GetReviewByIdEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewById(Guid id)
        {
            try
            {
                var reviews = await _reviewService.GetById(id);
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Review {id} retrieved successfully",
                    IsSuccess = true,
                    Data = reviews
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpPost(ApiEndpointConstants.Review.CreateReviewEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateReview(CreateReviewRequest request)
        {
            try
            {
                var reviews = await _reviewService.Create(request);
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Review created successfully",
                    IsSuccess = true,
                    Data = reviews
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpPut(ApiEndpointConstants.Review.RespondReviewEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RespondReview(Guid id, RespondReviewRequest request)
        {
            try
            {
                var reviews = await _reviewService.Update(id, request);
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Review updated successfully",
                    IsSuccess = true,
                    Data = reviews
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpPut(ApiEndpointConstants.Review.DeleteReviewEndpoint)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            try
            {
                var reviews = await _reviewService.Delete(id);
                var apiResponse = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Review deleted successfully",
                    IsSuccess = true,
                    Data = reviews
                };
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
