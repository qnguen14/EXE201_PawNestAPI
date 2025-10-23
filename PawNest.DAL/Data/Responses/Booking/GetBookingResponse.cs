using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.DAL.Data.Responses.Pet;

namespace PawNest.DAL.Data.Responses.Booking
{
    public class GetBookingResponse
    {
        public Guid BookingId { get; set; }
        public TimeOnly PickUpTime { get; set; }
        public DateOnly BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public Guid ServiceId { get; set; }
        public Guid FreelancerId { get; set; }
        public Guid CustomerId { get; set; }
        public List<GetPetResponse> Pets { get; set; } = new List<GetPetResponse>();
    }
}
