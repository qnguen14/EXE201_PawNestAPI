using PawNest.Repository.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Requests.Post
{
    public class CreatePostRequest
    {
        [Required(ErrorMessage="Title for post is required")]
        [MaxLength(50,ErrorMessage ="Title cannot contain more than 50 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content for post is required")]
        [MaxLength(200,ErrorMessage ="Content cannot exceed 200 characters")]
        public string Content { get; set; }

        public string ImageUrl { get; set; }
        [Required(ErrorMessage ="Post status is required")]
        public PostStatus Status { get; set; } = PostStatus.Pending;
        [Required(ErrorMessage = "Post category is required")]
        public PostCategory Category { get; set; }
    }
}
