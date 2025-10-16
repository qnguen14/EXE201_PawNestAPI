using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public TimeOnly PickpTime { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateOnly BookingDate { get; set; }
        public BookingStatus Status { get; set; }

        // FK
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }

        public Guid FreelancerId { get; set; }
        public User Freelancer { get; set; } = null!;

        public Guid CustomerId { get; set; }
        public User Customer { get; set; } = null!;

        public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}
