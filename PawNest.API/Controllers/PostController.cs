using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Metadata;
using PawNest.Repository.Data.Requests.Post;
using PawNest.Repository.Data.Responses.Post;
using PawNest.Services.Services.Interfaces;

namespace PawNest.API.Controllers
{
    [ApiController]
    [Route(ApiEndpointConstants.Post.PostEndpoint)]
    
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;    

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả các bài đăng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin, Staff, Freelancer,Customer")]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _postService.GetAllPosts();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
        /// <summary>
        /// Tạo bài đăng mới
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdPost = await _postService.CreatePost(request);

                var apiResponse = new ApiResponse<CreatePostResponse>
                {
                    Message = "Posts retrieved successfully.", // "Login successful"
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = createdPost 
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
        
        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePostAdmin([FromBody] CreatePostRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _postService.CreatePostAdmin(request);

                if (!success)
                {
                    return BadRequest("Unable to create post as admin");
                }

                var apiResponse = new ApiResponse<bool>
                {
                    Message = "Post created successfully.",
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = true
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        /// <summary>
        /// Xóa bài đăng theo ID
        /// </summary>
        [HttpDelete("{postId:guid}")]
        [Authorize(Roles = "Admin, Staff, Freelancer,Customer")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            try
            {
                await _postService.DeletePost(postId);
                return Ok("Post deleted successfully");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
        /// <summary>
        /// Lấy thông tin bài đăng theo ID
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [HttpGet("{postId:guid}")]
        [Authorize(Roles = "Admin, Staff, Freelancer,Customer")]
        public async Task<IActionResult> GetPostById(Guid postId)
        {
            try
            {
                var post = await _postService.GetPostById(postId);
                return Ok(post);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }
        /// <summary>
        /// Cập nhật bài đăng theo ID
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{postId:guid}")]
        [Authorize(Roles = "Admin, Staff, Freelancer,Customer")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedPost = await _postService.UpdatePost(request, postId);

                return Ok(updatedPost);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        /// <summary>
        /// Lấy các bài đăng của staff hiện tại
        /// </summary>
        [HttpGet("staff")]
        [Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> GetPostByStaffId()
        {
            try
            {
                var posts = await _postService.GetPostByStaffId();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        /// <summary>
        /// Lấy các bài đăng đang chờ duyệt
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> GetAllPendingPosts()
        {
            try
            {
                var posts = await _postService.GetAllPendingPosts();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        /// <summary>
        /// Duyệt bài đăng theo ID
        /// </summary>
        [HttpPost("{postId:guid}/approve")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> ApprovePost(Guid postId)
        {
            try
            {
                var result = await _postService.ApprovePost(postId);
                if (!result)
                {
                    return BadRequest("Unable to approve post");
                }

                var apiResponse = new ApiResponse<bool>
                {
                    Message = "Post approved successfully.",
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                };

                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        /// <summary>
        /// Từ chối bài đăng theo ID
        /// </summary>
        [HttpPost("{postId:guid}/reject")]
        [Authorize(Roles = "Admin, Staff")]
        public async Task<IActionResult> RejectPost(Guid postId)
        {
            try
            {
                var result = await _postService.RejectPost(postId);
                if (!result)
                {
                    return BadRequest("Unable to reject post");
                }

                var apiResponse = new ApiResponse<bool>
                {
                    Message = "Post rejected successfully.",
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                };

                return Ok(apiResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

    }
}
