using PawNest.DAL.Data.Requests.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetProfileAsync(Guid userId);
    }
}
