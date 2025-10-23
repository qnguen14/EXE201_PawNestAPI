using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Responses.User
{
    public class GetFreelancerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string AvatarUrl { get; set; }
        public List<Entities.Service> Services { get; set; } = [];
        public List<Review> ReviewsReceived { get; set; } = new List<Review>();
    }
}
