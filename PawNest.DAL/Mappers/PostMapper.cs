using Riok.Mapperly.Abstractions;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Post;
using PawNest.DAL.Data.Responses.Post;

namespace PawNest.DAL.Mappers;

[Mapper]
public partial class PostMapper
{
    // CreatePostRequest to Post
    public partial Post MapToPost(CreatePostRequest request);
    
    // Post to CreatePostResponse
    [MapProperty(nameof(Post.Id), nameof(CreatePostResponse.PostId))]
    [MapProperty(nameof(Post.Status), nameof(CreatePostResponse.PostStatus))]
    [MapProperty(nameof(Post.Category), nameof(CreatePostResponse.PostCategory))]
    public partial CreatePostResponse MapToCreatePostResponse(Post post);
    
    // IEnumerable mapping
    public partial IEnumerable<CreatePostResponse> MapToCreatePostResponseList(IEnumerable<Post> posts);
    
    // For update scenario
    public partial void UpdatePostFromRequest(Post source, Post target);
}
