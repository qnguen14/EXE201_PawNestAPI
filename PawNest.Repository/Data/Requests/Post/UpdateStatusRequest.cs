using PawNest.Repository.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Requests.Post
{
    public class UpdateStatusRequest
    {
        [Required]
        public PostStatus Status { get; set; }
    }
}
