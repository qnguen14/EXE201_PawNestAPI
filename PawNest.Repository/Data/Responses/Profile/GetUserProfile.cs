using PawNest.Repository.Data.Responses.Booking;
using PawNest.Repository.Data.Responses.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Responses.Profile
{
    public class GetUserProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; }

        public List<GetBookingResponse> Bookings { get; set; } = new List<GetBookingResponse>();
        public List<GetPetResponse> Pets { get; set; } = new List<GetPetResponse>();

        //TODO: add user's Bookings, Pets, and ReviewsWritten
    }
}
