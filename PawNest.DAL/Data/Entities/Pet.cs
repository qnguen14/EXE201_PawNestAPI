using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Entities
{
    public class Pet
    {
        [Key]
        [Required]
        public Guid PetId { get; set; }
        [Required]
        [StringLength(30)]
        public string PetName { get; set; }
        public string Species { get; set; }
        public string Breed { get; set; }

        public Guid CustomerId { get; set; }
        public User Customer { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
