using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Entities
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }

    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; }

        public TimeOnly PickUpTime { get; set; }

        public DateOnly BookingDate { get; set; }

        public BookingStatus Status { get; set; }

        public decimal TotalPrice { get; set; }

        public bool IsPaid { get; set; }

        public string? Notes { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public virtual User Freelancer { get; set; } = null!;

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public virtual User Customer { get; set; } = null!;

        // Navigation Properties
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
        public virtual Payment? Payment { get; set; }
    }
}