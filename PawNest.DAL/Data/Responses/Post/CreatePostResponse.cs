using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Responses.Post
{
    public class CreatePostResponse
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public PostStatus PostStatus { get; set; }
        public PostCategory PostCategory { get; set; }
        public Guid StaffId { get; set; }
    }
}
