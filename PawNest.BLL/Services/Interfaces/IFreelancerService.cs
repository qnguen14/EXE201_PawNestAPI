using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Responses.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IFreelancerService
    {
        Task<IEnumerable<GetFreelancerResponse>> GetAllFreelancersAsync();
        Task<GetFreelancerResponse> GetFreelancerByIdAsync(Guid id);
        Task<IEnumerable<GetFreelancerResponse>> SearchFreelancers(string address, string serviceName);
        Task<IEnumerable<GetFreelancerResponse>> SortFreelancers(string serviceName, string minPrice, string maxPrice); 
    }
}
