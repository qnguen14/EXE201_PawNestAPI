using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Responses.User;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.DAL.Mappers;

namespace PawNest.BLL.Services.Implements
{
    public class FreelancerService : BaseService<FreelancerService>, IFreelancerService
    {
        private readonly UserMapper _userMapper;
        
        public FreelancerService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<FreelancerService> logger, UserMapper userMapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userMapper = userMapper;
        }

        // Implement methods related to freelancer operations
        public async Task<IEnumerable<GetFreelancerResponse>> GetAllFreelancersAsync()
        {
            try
            {
                var freelancers = await _unitOfWork.GetRepository<User>()
                    .GetListAsync(
                        predicate: u => u.RoleId == 3,
                        include: source => source
                            .Include(u => u.Services)
                            .Include(u => u.ReviewsReceived),
                        orderBy: u => u.OrderBy(n => n.Name)
                    );

                if (freelancers == null || !freelancers.Any())
                {
                    _logger.LogWarning("No freelancers found.");
                    return Enumerable.Empty<GetFreelancerResponse>();
                }

                var freelancersResponse = freelancers.Select(f => _userMapper.MapToGetFreelancerResponse(f));
                return freelancersResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving freelancers: " + ex.Message);
                throw;
            }
        }

        public async Task<GetFreelancerResponse> GetFreelancerByIdAsync(Guid id)
        {
            try
            {
                var freelancers = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(
                        predicate: u => u.RoleId == 3,
                        include: source => source
                            .Include(u => u.Services)
                            .Include(u => u.ReviewsReceived),
                        orderBy: u => u.OrderBy(n => n.Name)
                    );

                if (freelancers == null)
                {
                    _logger.LogWarning("No freelancers found.");
                    return null;
                }

                var freelancerResponse = _userMapper.MapToGetFreelancerResponse(freelancers);
                return freelancerResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving freelancers: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<GetFreelancerResponse>> SearchFreelancers(string address, string serviceName)
        {
            try
            {
                var freelancers = await _unitOfWork.GetRepository<User>()
                    .GetListAsync(
                        predicate: u => u.RoleId == 3 &&
                                       (string.IsNullOrEmpty(address) || u.Address.Contains(address)) &&
                                       (string.IsNullOrEmpty(serviceName) || u.Services.Any(s => s.Type.ToString().Contains(serviceName))),
                        include: source => source
                            .Include(u => u.Services)
                            .Include(u => u.ReviewsReceived),
                        orderBy: u => u.OrderBy(n => n.Name)
                    );

                if (freelancers == null || !freelancers.Any())
                {
                    return null;
                }

                var freelancersResponse = freelancers.Select(f => _userMapper.MapToGetFreelancerResponse(f));
                return freelancersResponse;

            } catch (Exception ex)
            {
                _logger.LogError("An error occurred while searching freelancers: " + ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<GetFreelancerResponse>> SortFreelancers(string serviceName, string minPrice, string maxPrice)
        {
            try
            {
                var freelancers = await _unitOfWork.GetRepository<User>()
                .GetListAsync(
                    predicate: u => u.RoleId == 3 &&
                                   (string.IsNullOrEmpty(serviceName) || u.Services.Any(s => s.Type.ToString().Contains(serviceName))) &&
                                   (string.IsNullOrEmpty(minPrice) || u.Services.Any(s => s.Price >= Convert.ToDecimal(minPrice))) &&
                                   (string.IsNullOrEmpty(maxPrice) || u.Services.Any(s => s.Price <= Convert.ToDecimal(maxPrice))),
                    include: source => source
                        .Include(u => u.Services)
                        .Include(u => u.ReviewsReceived),
                    orderBy: u => u.OrderBy(n => n.Name)
                );

                if (freelancers == null || !freelancers.Any())
                {
                    return null;
                }

                var freelancersResponse = freelancers.Select(f => _userMapper.MapToGetFreelancerResponse(f));
                return freelancersResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while sorting freelancers: " + ex.Message);
                throw;
            }
        }
    }
}
