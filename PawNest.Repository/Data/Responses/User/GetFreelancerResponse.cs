using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Responses.Review;
using PawNest.Repository.Data.Responses.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Responses.User
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
        public List<GetServiceResponse> Services { get; set; } = [];
        public List<GetReviewResponse> ReviewsReceived { get; set; } = [];
    }
}
