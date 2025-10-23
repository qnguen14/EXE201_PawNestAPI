using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Requests.Booking
{
    public class UpdateBookingRequest
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public Guid ServiceId { get; set; }
        public List<CreatePetRequest> Pets { get; set; } = new List<CreatePetRequest>();
    }
}
