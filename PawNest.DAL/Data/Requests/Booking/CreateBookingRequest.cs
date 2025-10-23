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
        public TimeOnly PickupTime { get; set; }
        public DateOnly BookingDate { get; set; }
        public Guid ServiceId { get; set; }
        public Guid FreelancerId { get; set; }
        public List<Guid> PetId { get; set; } = new List<Guid>();
    }
}
