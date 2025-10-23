using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Responses.Profile
{
    public class GetUserProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? AvatarUrl { get; set; } = null;
        public string Role { get; set; }
        public bool IsActive { get; set; }
        
        //TODO: add user's Bookings, Pets, and ReviewsWritten
    }
}
