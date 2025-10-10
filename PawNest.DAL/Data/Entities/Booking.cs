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
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BookingStatus Status { get; set; }

        // FK
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }

        public Guid PetId { get; set; }
        public Pet Pet { get; set; } = null!;

        public Guid CustomerId { get; set; }
        public User Customer { get; set; } = null!;
    }
}
