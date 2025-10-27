using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string PetName { get; set; } = null!;

        [Required]
        public string Species { get; set; } = null!;

        public string? Breed { get; set; }

        // Foreign Key
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public virtual User Customer { get; set; } = null!;

        // Navigation Property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}