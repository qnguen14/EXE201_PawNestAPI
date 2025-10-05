using AutoMapper;
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

namespace PawNest.BLL.Services.Implements
{
    public class FreelancerService : BaseService<FreelancerService>, IFreelancerService
    {
        public FreelancerService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<FreelancerService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
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

                var freelancersResponse = _mapper.Map<IEnumerable<GetFreelancerResponse>>(freelancers);
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

                var freelancerResponse = _mapper.Map<GetFreelancerResponse>(freelancers);
                return freelancerResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving freelancers: " + ex.Message);
                throw;
            }
        }
    }
}
