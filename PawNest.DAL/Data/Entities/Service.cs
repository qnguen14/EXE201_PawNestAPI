using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Title { get; set; }
        public string? Description { get; set; } 
        public ServiceType Type { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }

        // FK
        public Guid FreelancerId { get; set; }
        public User Freelancer { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
