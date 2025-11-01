using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.Repository.Data.Responses.Profile;

namespace PawNest.Services.Services.Interfaces
{
    public interface IProfileService
    {
        Task<GetUserProfile> GetProfileAsync();
    }
}
