using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Requests.Booking
{
    public class UpdateBookingRequest
    {
        public PickUpTime PickUpTime { get; set; }
        public PickUpStatus PickUpStatus { get; set; }
        public DateOnly BookingDate { get; set; }
        public BookingStatus Status { get; set; }
        public Guid ServiceId { get; set; }
        public List<CreatePetRequest> Pets { get; set; } = new List<CreatePetRequest>();
    }
}
