using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Responses.Pet;
using PawNest.Repository.Data.Responses.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<CreatePostResponse>> GetAllPosts();
        Task<CreatePostResponse> GetPostById(Guid postId);
        Task<CreatePostResponse> CreatePost(Post post);
        Task<CreatePostResponse> UpdatePost(Post post);
        Task<bool> DeletePost(Guid postId);
        Task<IEnumerable<CreatePostResponse>> GetPostsByStaffId();
        Task<IEnumerable<CreatePostResponse>> GetAllPendingPosts();
        Task<IEnumerable<CreatePostResponse>> GetFilteredPosts(string filter); // TODO: use poststatus enum filter

        // TODO: add paging to posts
    }
}
