using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Service;
using PawNest.Repository.Data.Responses.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Interfaces
{
    public interface IServiceService
    {
        Task<IEnumerable<GetServiceResponse>> GetAllServicesAsync();
        Task<GetServiceResponse> GetServiceByIdAsync(Guid serviceId);
        Task<GetServiceResponse> CreateServiceAsync(CreateServiceRequest request);
        Task<GetServiceResponse> UpdateServiceAsync(Guid serviceId, UpdateServiceRequest request);
        Task<bool> DeleteServiceAsync(Guid serviceId);
    }
}
