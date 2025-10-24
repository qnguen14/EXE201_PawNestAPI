using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Requests.Booking
{
    public class CreateBookingRequest
    {
        public TimeOnly PickUpTime { get; set; }
        public DateOnly BookingDate { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public List<Guid> ServiceIds { get; set; } = new List<Guid>();
        public Guid FreelancerId { get; set; }
        public List<Guid> PetIds { get; set; } = new List<Guid>();
    }
}
