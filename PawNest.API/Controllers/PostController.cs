
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.Services.Services.Implements;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Requests.Pet;
using PawNest.Repository.Data.Requests.Post;
using PawNest.Repository.Data.Responses.Pet;
using PawNest.Repository.Data.Responses.Post;
using PawNest.Repository.Mappers;

namespace PawNest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;
        private readonly IMapperlyMapper _postMapper;    

        public PostController(IPostService postService, ILogger<PostController> logger, IMapperlyMapper mapper)
        {
            _postService = postService;
            _logger = logger;
            _postMapper = mapper;
        }

        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var post = _postMapper.MapToPost(request);
                var createdPost = await _postService.CreatePost(post);
                var response = _postMapper.MapToCreatePostResponse(createdPost);
                
                return CreatedAtAction(nameof(GetPostById), new { postId = response.PostId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("{postId:guid}")]
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

        [HttpGet("{postId:guid}")]
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

        [HttpPut("{postId:guid}")]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] CreatePostRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var post = _postMapper.MapToPost(request);
                post.Id = postId;

                var updatedPost = await _postService.UpdatePost(post);
                var response = _postMapper.MapToCreatePostResponse(updatedPost);

                return Ok(response);
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
