 using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Responses.Pet;
using PawNest.Repository.Data.Responses.Post;
using PawNest.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.Repository.Mappers;
using PawNest.Repository.Data.Requests.Post;

namespace PawNest.Services.Services.Implements
{
    public class PostService : BaseService<PostService>, IPostService
    {
        private readonly IMapperlyMapper _postMapper;
        public PostService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<PostService> logger, IHttpContextAccessor httpContextAccessor, IMapperlyMapper mapper) : base(unitOfWork, logger, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _postMapper = mapper;
        }

        public async Task<CreatePostResponse> CreatePost(CreatePostRequest request)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var existingPost = await _unitOfWork.GetRepository<Post>()
                        .FirstOrDefaultAsync(
                            predicate: p => p.Title == request.Title,
                            orderBy: null,
                            include: null
                        );

                    if (existingPost != null)
                    {
                        throw new BadRequestException($"A post with the title '{request.Title}' already exists.");
                    }

                    var post = _postMapper.MapToPost(request);
                    var now = DateTime.UtcNow;
                    post.CreatedAt = now;
                    post.UpdatedAt = now;
                    await _unitOfWork.GetRepository<Post>().InsertAsync(post);

                    return _postMapper.MapToCreatePostResponse(post);

                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the post: " + ex.Message);
                throw;
            }
        }

        public async Task<CreatePostResponse> UpdatePost(UpdatePostRequest request, Guid postId)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var post = await _unitOfWork.GetRepository<Post>()
                        .FirstOrDefaultAsync(predicate: p => p.Id == postId);

                    if (post == null)
                        throw new NotFoundException($"Post with ID {postId} not found.");

                    // Map Request → Post
                    _postMapper.UpdatePostFromRequest(request, post);

                    post.UpdatedAt = DateTime.UtcNow;

                     _unitOfWork.GetRepository<Post>().UpdateAsync(post);

                    return _postMapper.MapToCreatePostResponse(post);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while updating post: " + ex.Message);
                throw;
            }
        }

        public async Task<bool> DeletePost(Guid postId)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
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
                    return true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the post: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CreatePostResponse>> GetAllPosts()
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var posts = await _unitOfWork.GetRepository<Post>()
                        .GetListAsync(
                            predicate: null,
                            orderBy: p => p.OrderBy(x => x.CreatedAt),
                            include: null
                        );
                    return posts.Select(post => _postMapper.MapToCreatePostResponse(post)).ToList();
                });
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
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
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
                    return _postMapper.MapToCreatePostResponse(post);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the post: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CreatePostResponse>> GetPostByStaffId()
        {
            try
            {
                var role = GetCurrentUserRole();
                var userId = GetCurrentUserId();

                if (role != "Staff")
                {
                    throw new UnauthorizedException("Only staff members view these posts.");
                }

                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var posts = await _unitOfWork.GetRepository<Post>()
                        .GetListAsync(
                            predicate: p => p.StaffId == userId,
                            orderBy: p => p.OrderBy(x => x.CreatedAt),
                            include: null
                        );
                    return posts.Select(post => _postMapper.MapToCreatePostResponse(post)).ToList();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving posts by staff ID: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CreatePostResponse>> GetAllPendingPosts()
        {
            try
            {
                var role = GetCurrentUserRole();
                var userId = GetCurrentUserId();

                if (role != "Staff")
                {
                    throw new UnauthorizedException("Only staff members view these posts.");
                }

                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var posts = await _unitOfWork.GetRepository<Post>()
                        .GetListAsync(
                            predicate: p => p.StaffId == userId,
                            orderBy: p => p.OrderBy(x => x.CreatedAt),
                            include: null
                        );
                    return posts.Select(post => _postMapper.MapToCreatePostResponse(post)).ToList();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving posts by staff ID: " + ex.Message);
                throw;
            }
        }
    }
}
