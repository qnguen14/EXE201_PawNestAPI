using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Responses.Booking
{
    public class GetBookingUpdateResponse
    {
        public Guid BookingId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateOnly BookingDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BookingStatus Status { get; set; }
        public Guid ServiceId { get; set; }
        public Guid FreelancerId { get; set; }
        public Guid CustomerId { get; set; }
        public List<CreatePetRequest> Pets { get; set; } = new List<CreatePetRequest>();
    }
}
