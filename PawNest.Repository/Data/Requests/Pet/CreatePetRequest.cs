using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Requests.Pet
{
    public class CreatePetRequest
    {
        [Required(ErrorMessage = "Pet name is required")]
        [MaxLength(30, ErrorMessage = "Pet name cannot contain more than 30 characters")]
        public string PetName { get; set; }

        [MaxLength(45, ErrorMessage = "Pet species cannot contain more than 45 characters")]
        public string Species { get; set; }

        [MaxLength(45, ErrorMessage = "Pet breed cannot contain more than 45 characters")]
        public string Breed { get; set; }
    }

    public class UpdatedPetRequest
    {
        [Required(ErrorMessage = "Pet name is required")]
        [MaxLength(30, ErrorMessage = "Pet name cannot contain more than 30 characters")]
        public string PetName { get; set; }

        [MaxLength(45, ErrorMessage = "Pet species cannot contain more than 45 characters")]
        public string Species { get; set; }

        [MaxLength(45, ErrorMessage = "Pet breed cannot contain more than 45 characters")]
        public string Breed { get; set; }

        [Required(ErrorMessage = "Owner ID is required")]
        public Guid CustomerId { get; set; }
    }
}
