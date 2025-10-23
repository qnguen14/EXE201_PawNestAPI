using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.BLL.Services.Implements;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Requests.Pet;
using PawNest.DAL.Data.Requests.Post;
using PawNest.DAL.Data.Responses.Pet;
using PawNest.DAL.Data.Responses.Post;

namespace PawNest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, IMapper mapper, ILogger<PostController> logger)
        {
            _postService = postService;
            _mapper = mapper;
            _logger = logger;
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

                var post = _mapper.Map<Post>(request);
                var createdPost = await _postService.CreatePost(post);
                var response = _mapper.Map<CreatePostResponse>(createdPost);

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

                var post= _mapper.Map<Post>(request);
                post.Id = postId;

                var updatedPost = await _postService.UpdatePost(post);
                var response = _mapper.Map<CreatePostResponse>(updatedPost);

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
