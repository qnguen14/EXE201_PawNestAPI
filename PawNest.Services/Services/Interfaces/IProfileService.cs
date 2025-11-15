using PawNest.Repository.Data.Requests.Profile;
using PawNest.Repository.Data.Responses.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Interfaces
{
    public interface IProfileService
    {
        Task<GetUserProfile> GetProfileAsync();
        Task<GetFreelancerProfile> GetFreelancerProfileAsync();

        // TODO Update profile, upload profile picture, etc.
        Task<GetUserProfile> UpdateUserProfileAsync(UpdateUserProfileRequest request);
        Task<GetFreelancerProfile> UpdateFreelancerProfileAsync(UpdateFreelancerProfileRequest request);
    }
}
