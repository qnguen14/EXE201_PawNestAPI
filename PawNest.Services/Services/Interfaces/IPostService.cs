using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Post;
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
        Task<CreatePostResponse> CreatePost(CreatePostRequest request);
        Task<CreatePostResponse> UpdatePost(UpdatePostRequest request, Guid id);
        Task<bool> DeletePost(Guid postId);
        Task<IEnumerable<CreatePostResponse>> GetPostByStaffId();
        Task<IEnumerable<CreatePostResponse>> GetAllPendingPosts();
        //Task<IEnumerable<CreatePostResponse>> GetFilteredPosts(string filter); // TODO: use poststatus enum filter

        // TODO: add paging to posts
    }
}
