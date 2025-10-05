using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.DTO
{
    public class PetDTO
    {
        public class CreatePetRequest
        {
            [Required]
            [StringLength(30)]
            public string PetName { get; set; }

            [Required]
            public string Species { get; set; }

            public string? Breed { get; set; }

            // Không cần OwnerId vì lấy từ JWT token
        }

        public class UpdatePetRequest
        {
            [Required]
            public Guid PetId { get; set; }

            [Required]
            [StringLength(30)]
            public string PetName { get; set; }

            [Required]
            public string Species { get; set; }

            public string? Breed { get; set; }
        }

        public class PetResponse
        {
            public Guid PetId { get; set; }
            public string PetName { get; set; }
            public string Species { get; set; }
            public string? Breed { get; set; }
            public Guid OwnerId { get; set; }
            public string? OwnerName { get; set; } // Chỉ trả về tên, không trả toàn bộ Owner
        }
    }
}
