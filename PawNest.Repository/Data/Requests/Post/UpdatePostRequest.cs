using PawNest.Repository.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Requests.Post
{
    public class UpdatePostRequest
    {
        [Required]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Required] 
        [Column("PostCategory")]
        [EnumDataType(typeof(PostCategory))]
        public PostCategory Category { get; set; }

    }
}
