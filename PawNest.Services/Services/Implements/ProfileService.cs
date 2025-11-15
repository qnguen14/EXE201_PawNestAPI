using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Requests.Profile;
using PawNest.Repository.Data.Responses.Profile;
using PawNest.Repository.Mappers;
using PawNest.Repository.Repositories.Interfaces;
using PawNest.Services.Services.Interfaces;

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
                bool verify = await IsFreelancer(userId);

                if (!verify)
                {
                    throw new UnauthorizedAccessException("User is not a freelancer.");
                }

                var userRepo = _unitOfWork.GetRepository<User>();

                var user = await userRepo.FirstOrDefaultAsync(
                    predicate: u => u.Id == userId && u.IsActive,
                    orderBy: null,
                    include: q => q.Include(u => u.Role)
                                   .Include(u => u.ReviewsReceived)
                                   .Include(u => u.Bookings.Where(b => b.FreelancerId == userId))
                                       .ThenInclude(b => b.Customer)
                                   .Include(u => u.Bookings.Where(b => b.FreelancerId == userId))
                                       .ThenInclude(b => b.Pets)
                                   .Include(u => u.Bookings.Where(b => b.FreelancerId == userId))
                                       .ThenInclude(b => b.Services)
                                   .Include(u => u.Services.Where(s => s.FreelancerId == userId))
                );

                if (user == null)
                {
                    throw new NotFoundException("User not found.");
                }

                // THAY ĐỔI: Dùng helper method
                return _profileMapper.MapToGetFreelancerProfileWithBookings(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting freelancer profile: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<GetUserProfile> GetProfileAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                bool verify = await IsCustomer(userId);

                if (!verify)
                {
                    throw new UnauthorizedAccessException("User is not a customer.");
                }

                var userRepo = _unitOfWork.GetRepository<User>();

                var user = await userRepo.FirstOrDefaultAsync(
                    predicate: u => u.Id == userId && u.IsActive,
                    orderBy: null,
                    include: q => q.Include(u => u.Role)
                                   .Include(u => u.Pets)
                                   .Include(u => u.ReviewsWritten)
                                   .Include(u => u.Bookings.Where(b => b.CustomerId == userId))
                                       .ThenInclude(b => b.Freelancer)
                                   .Include(u => u.Bookings.Where(b => b.CustomerId == userId))
                                       .ThenInclude(b => b.Pets)
                                   .Include(u => u.Bookings.Where(b => b.CustomerId == userId))
                                       .ThenInclude(b => b.Services)
                );

                if (user == null)
                {
                    throw new NotFoundException("User not found.");
                }

                // THAY ĐỔI: Dùng helper method
                return _profileMapper.MapToGetUserProfileWithBookings(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<GetUserProfile> UpdateUserProfileAsync(UpdateUserProfileRequest request)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var userId = GetCurrentUserId();
                    bool verify = await IsCustomer(userId);

                    if (!verify)
                    {
                        throw new UnauthorizedAccessException("User is not a customer.");
                    }

                    var userRepo = _unitOfWork.GetRepository<User>();

                    var user = await userRepo.FirstOrDefaultAsync(
                        predicate: u => u.Id == userId && u.IsActive,
                        include: q => q.Include(u => u.Role)
                                       .Include(u => u.Pets)
                                       .Include(u => u.ReviewsWritten)
                                       .Include(u => u.Bookings.Where(b => b.CustomerId == userId))
                                           .ThenInclude(b => b.Freelancer)
                                       .Include(u => u.Bookings.Where(b => b.CustomerId == userId))
                                           .ThenInclude(b => b.Pets)
                                       .Include(u => u.Bookings.Where(b => b.CustomerId == userId))
                                           .ThenInclude(b => b.Services)
                    );

                    if (user == null)
                    {
                        throw new NotFoundException("User not found.");
                    }

                    _profileMapper.UpdateUserProfileFromRequest(request, user);
                    userRepo.UpdateAsync(user);

                    // THAY ĐỔI: Dùng helper method
                    return _profileMapper.MapToGetUserProfileWithBookings(user);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user profile: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<GetFreelancerProfile> UpdateFreelancerProfileAsync(UpdateFreelancerProfileRequest request)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var userId = GetCurrentUserId();
                    bool verify = await IsFreelancer(userId);

                    if (!verify)
                    {
                        throw new UnauthorizedAccessException("User is not a freelancer.");
                    }

                    var userRepo = _unitOfWork.GetRepository<User>();

                    var user = await userRepo.FirstOrDefaultAsync(
                        predicate: u => u.Id == userId && u.IsActive,
                        include: q => q.Include(u => u.Role)
                                       .Include(u => u.ReviewsReceived)
                                       .Include(u => u.Bookings.Where(b => b.FreelancerId == userId))
                                           .ThenInclude(b => b.Customer)
                                       .Include(u => u.Bookings.Where(b => b.FreelancerId == userId))
                                           .ThenInclude(b => b.Pets)
                                       .Include(u => u.Bookings.Where(b => b.FreelancerId == userId))
                                           .ThenInclude(b => b.Services)
                                       .Include(u => u.Services.Where(s => s.FreelancerId == userId))
                    );

                    if (user == null)
                    {
                        throw new NotFoundException("User not found.");
                    }

                    _profileMapper.UpdateFreelancerProfileFromRequest(request, user);
                    userRepo.UpdateAsync(user);

                    // THAY ĐỔI: Dùng helper method
                    return _profileMapper.MapToGetFreelancerProfileWithBookings(user);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating freelancer profile: {Message}", ex.Message);
                throw;
            }
        }
    }
}
