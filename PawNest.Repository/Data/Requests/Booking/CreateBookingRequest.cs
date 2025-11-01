using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Requests.Booking
{
    public class CreateBookingRequest
    {
        public PickUpTime PickUpTime { get; set; }
        public DateOnly BookingDate { get; set; }
        public List<Guid> ServiceIds { get; set; } = new List<Guid>();
        public Guid FreelancerId { get; set; }
        public List<Guid> PetIds { get; set; } = new List<Guid>();
    }
}
