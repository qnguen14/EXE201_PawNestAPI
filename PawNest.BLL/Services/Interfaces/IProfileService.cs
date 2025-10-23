using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.DAL.Data.Responses.Profile;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IProfileService
    {
        Task<GetUserProfile> GetProfileAsync();
    }
}
