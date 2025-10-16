using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Service;
using PawNest.DAL.Data.Responses.Service;
using PawNest.DAL.Repositories.Implements;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Implements
{
    public class ServiceService : BaseService<ServiceService>, IServiceService
    {
        public ServiceService(
            IUnitOfWork<PawNestDbContext> unitOfWork,
            ILogger<ServiceService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        private void IsFreelancer()
        {
            var userRole = GetCurrentUserRole();
            if (userRole != "Freelancer")
            {
                throw new UnauthorizedAccessException("Only freelancers can perform this action.");
            }
        }

        public async Task<GetServiceResponse> CreateServiceAsync(CreateServiceRequest request)
        {
            IsFreelancer();
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var existingService = await _unitOfWork.GetRepository<Service>().FirstOrDefaultAsync(predicate: s => s.Title == request.Title && s.FreelancerId == request.FreelancerId);

                    if (existingService  != null)
                    {
                        throw new InvalidOperationException("A service with the same title already exists for this freelancer.");
                    }

                    var service = _mapper.Map<Service>(request);
                    service.FreelancerId = GetCurrentUserId();
                    await _unitOfWork.GetRepository<Service>().InsertAsync(service);

                    return _mapper.Map<GetServiceResponse>(service);
                });

            } catch (Exception ex)
            {
                _logger.LogError("An error occurred while creating the service: " + ex.Message);
                throw;
            }
        }

        public Task<bool> DeleteServiceAsync(Guid serviceId)
        {
            IsFreelancer();
            try
            {
                return _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var service = await _unitOfWork.GetRepository<Service>().FirstOrDefaultAsync(predicate: s => s.ServiceId == serviceId && s.FreelancerId == GetCurrentUserId());
                    if (service == null)
                    {
                        throw new KeyNotFoundException("Service not found or you do not have permission to delete this service.");
                    }
                    _unitOfWork.GetRepository<Service>().DeleteAsync(service);
                    return true;
                });

            } catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the service: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<GetServiceResponse>> GetAllServicesAsync()
        {
            try
            {
                var services = await _unitOfWork.GetRepository<Service>()
                    .GetListAsync(null, 
                                  include: s => s.Include(u => u.Bookings),
                                  orderBy: s => s.OrderBy(u => u.ServiceId));
                return _mapper.Map<IEnumerable<GetServiceResponse>>(services);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving services: " + ex.Message);
                throw;
            }
        }

        public async Task<GetServiceResponse> GetServiceByIdAsync(Guid serviceId)
        {
            try
            {
                var service = await _unitOfWork
                    .GetRepository<Service>()
                    .FirstOrDefaultAsync
                    (predicate: s => s.ServiceId == serviceId,
                    include: s => s.Include(u => u.Bookings),
                    orderBy: s => s.OrderBy(u => u.ServiceId));
                return _mapper.Map<GetServiceResponse>(service);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving the service: " + ex.Message);
                throw;
            }
        }

        public async Task<GetServiceResponse> UpdateServiceAsync(Guid serviceId, UpdateServiceRequest request)
        {
            try
            {
                IsFreelancer();
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var service = await _unitOfWork.GetRepository<Service>().FirstOrDefaultAsync(predicate: s => s.ServiceId == serviceId && s.FreelancerId == GetCurrentUserId());
                    if (service == null)
                    {
                        throw new KeyNotFoundException("Service not found or you do not have permission to update this service.");
                    }
                    _mapper.Map(request, service);
                    _unitOfWork.GetRepository<Service>().UpdateAsync(service);
                    return _mapper.Map<GetServiceResponse>(service);
                });

            } catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating the service: " + ex.Message);
                throw;
            }
        }
    }
}
