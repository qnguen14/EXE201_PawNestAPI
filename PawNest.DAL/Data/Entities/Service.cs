using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Entities
{
    public enum ServiceType
    {
        Grooming,
        Training,
        Walking,
        Sitting
    }

    public class Service
    {
        [Key]
        public Guid ServiceId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public ServiceType Type { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Key
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public virtual User Freelancer { get; set; } = null!;

        // Navigation Property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}