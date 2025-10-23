 using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Responses.Pet;
using PawNest.DAL.Data.Responses.Post;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.DAL.Mappers;

namespace PawNest.BLL.Services.Implements
{
    public class PostService : BaseService<PostService>, IPostService
    {
        private readonly PostMapper _postMapper;
        public PostService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<PostService> logger, IHttpContextAccessor httpContextAccessor, PostMapper postMapper) : base(unitOfWork, logger, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _postMapper = postMapper;
        }

        public async Task<Post> CreatePost(Post post)
        {
            try
            {
                await _unitOfWork.GetRepository<Post>().InsertAsync(post);
                await _unitOfWork.SaveChangesAsync();
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the post: " + ex.Message);
                throw;
            }
        }

        public async Task<Post> UpdatePost(Post post)
        {
            try
            {
                var existingPost = await _unitOfWork.GetRepository<Post>()
                    .FirstOrDefaultAsync(
                        predicate: p => p.Id == post.Id,
                        orderBy: null,
                        include: null
                    );
                if (existingPost == null)
                {
                    throw new NotFoundException($"Post with ID {post.Id} not found.");
                }

                _postMapper.UpdatePostFromRequest(post, existingPost);
                _unitOfWork.GetRepository<Post>().UpdateAsync(existingPost);
                await _unitOfWork.SaveChangesAsync();
                return existingPost;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the post: " + ex.Message);
                throw;
            }
        }
        public async  Task<bool> DeletePost(Guid postId)
        {
            try
            {
                var post = await _unitOfWork.GetRepository<Post>()
                    .FirstOrDefaultAsync(
                        predicate: p => p.Id == postId,
                        orderBy: null,
                        include: null
                    );

                if (post == null)
                {
                    throw new NotFoundException($"Post with ID {postId} not found.");
                }

                _unitOfWork.GetRepository<Post>().DeleteAsync(post);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch(Exception ex) 
            {
                _logger.LogError("An error occurred while deleting the post: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CreatePostResponse>> GetAllPosts()
        {
            try
            {
                var posts = await _unitOfWork.GetRepository<Post>()
                    .GetListAsync(
                        predicate: null, // Get all posts
                        orderBy: p => p.OrderBy(n => n.CreatedAt)
                    );

                if (posts == null || !posts.Any())
                {
                    throw new NotFoundException("No posts found.");
                }

                return posts.Select(p => _postMapper.MapToCreatePostResponse(p));
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving posts: " + ex.Message);
                throw;
            }
        }

        public async Task<CreatePostResponse> GetPostById(Guid postId)
        {
            try
            {
                var post = await _unitOfWork.GetRepository<Post>()
                    .FirstOrDefaultAsync(
                        predicate: p => p.Id == postId
                    );

                if (post == null)
                {
                    throw new NotFoundException($"Post with ID {postId} not found.");
                }

                return _postMapper.MapToCreatePostResponse(post);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the post: " + ex.Message);
                throw;
            }
        }

        public Task<Pet> GetPostByStaffId(Guid staffId)
        {
            throw new NotImplementedException();
        }
    }
}
