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
        Task<Post> CreatePost(Post post);
        Task<Post> UpdatePost(Post post);
        Task<bool> DeletePost(Guid postId);
        Task<Pet> GetPostByStaffId(Guid staffId);
    }
}
