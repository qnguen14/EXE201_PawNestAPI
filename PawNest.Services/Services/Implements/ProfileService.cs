using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Repositories.Interfaces;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Responses.Profile;
using PawNest.Repository.Mappers;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace PawNest.Services.Services.Implements
{
    public class ProfileService : BaseService<ProfileService>, IProfileService
    {
        private readonly IUnitOfWork<PawNestDbContext> _unitOfWork;
        private readonly IMapperlyMapper _profileMapper;
        private readonly ILogger<ProfileService> _logger;
        private readonly IUserService _userService;

        public ProfileService
            (IUnitOfWork<PawNestDbContext> unitOfWork, 
             ILogger<ProfileService> logger, 
             IMapperlyMapper mapper, 
             IHttpContextAccessor httpContextAccessor,
             IUserService userService)
            : base(unitOfWork, logger, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _profileMapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        private async Task<bool> IsFreelancer(Guid userId)
        {
            var fetchedUser = await _userService.GetById(userId);
            if (fetchedUser.Role == "Freelancer")
            {
                return true;
            }
            return false;
        }

        private async Task<bool> IsCustomer(Guid userId)
        {
            var fetchedUser = await _userService.GetById(userId);
            if (fetchedUser.Role == "Customer")
            {
                return true;
            }
            return false;
        }

        public async Task<GetFreelancerProfile> GetFreelancerProfileAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                User? user = null;
                bool verify = await IsFreelancer(userId);

                if (verify)
                {
                    var userRepo = _unitOfWork.GetRepository<User>();

                    user = await userRepo.FirstOrDefaultAsync(
                        predicate: u => u.Id == userId && u.IsActive,
                        orderBy: null,
                        include: q => q.Include(u => u.Role)
                                       .Include(u => u.Bookings.Where(q => q.FreelancerId.Equals(userId)))
                    );

                    if (user == null)
                    {
                        throw new NotFoundException("User not found.");
                    }

                    return _profileMapper.MapToGetFreelancerProfile(user);
                }

                throw new UnauthorizedAccessException("User is not a freelancer.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetUserProfile> GetProfileAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                User? user = null;
                bool verify = await IsCustomer(userId);

                if (verify)
                {
                    var userRepo = _unitOfWork.GetRepository<User>();

                    user = await userRepo.FirstOrDefaultAsync(
                        predicate: u => u.Id == userId && u.IsActive,
                        orderBy: null,
                        include: q => q.Include(u => u.Role)
                                       .Include(u => u.Pets)
                                       .Include(u => u.Bookings.Where(q => q.CustomerId.Equals(userId)))
                    );

                    if (user == null)
                    {
                        throw new NotFoundException("User not found.");
                    }

                    return _profileMapper.MapToGetUserProfile(user!);
                } else
                {
                    throw new UnauthorizedAccessException("User is not a customer.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
