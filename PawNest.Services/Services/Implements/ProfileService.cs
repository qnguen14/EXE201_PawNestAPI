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
                if (!await IsFreelancer(userId))
                    throw new UnauthorizedAccessException("User is not a freelancer.");

                var userRepo = _unitOfWork.GetRepository<User>();
                var bookingRepo = _unitOfWork.GetRepository<Booking>();

                var user = await userRepo.FirstOrDefaultAsync(
                    predicate: u => u.Id == userId && u.IsActive,
                    include: q => q
                        .Include(u => u.Role)
                        .Include(u => u.Services)
                        .Include(u => u.ReviewsReceived)
                );

                if (user == null)
                    throw new NotFoundException("User not found.");

                var bookings = await bookingRepo.GetListAsync(
                    predicate: b => b.FreelancerId == userId,
                    include: q => q
                        .Include(b => b.Customer)
                        .Include(b => b.Pets)
                        .Include(b => b.Services)
                );

                user.Bookings = bookings.ToList();

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
                if (!await IsCustomer(userId))
                    throw new UnauthorizedAccessException("User is not a customer.");

                var userRepo = _unitOfWork.GetRepository<User>();
                var bookingRepo = _unitOfWork.GetRepository<Booking>();

                // Load user WITHOUT Bookings
                var user = await userRepo.FirstOrDefaultAsync(
                    predicate: u => u.Id == userId && u.IsActive,
                    include: q => q
                        .Include(u => u.Role)
                        .Include(u => u.Pets)
                        .Include(u => u.ReviewsWritten)
                );

                if (user == null)
                    throw new NotFoundException("User not found.");

                // Load Bookings bằng query riêng (FIX LỖI EF CORE)
                var bookings = await bookingRepo.GetListAsync(
                    predicate: b => b.CustomerId == userId,
                    include: q => q
                        .Include(b => b.Freelancer)
                        .Include(b => b.Pets)
                        .Include(b => b.Services)
                );

                user.Bookings = bookings.ToList();

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
                    if (!await IsCustomer(userId))
                        throw new UnauthorizedAccessException("User is not a customer.");

                    var userRepo = _unitOfWork.GetRepository<User>();
                    var bookingRepo = _unitOfWork.GetRepository<Booking>();

                    var user = await userRepo.FirstOrDefaultAsync(
                        predicate: u => u.Id == userId && u.IsActive,
                        include: q => q
                            .Include(u => u.Role)
                            .Include(u => u.Pets)
                    );

                    if (user == null)
                        throw new NotFoundException("User not found.");

                    _profileMapper.UpdateUserProfileFromRequest(request, user);
                    userRepo.UpdateAsync(user);

                    var bookings = await bookingRepo.GetListAsync(
                        predicate: b => b.CustomerId == userId,
                        include: q => q
                            .Include(b => b.Freelancer)
                            .Include(b => b.Pets)
                            .Include(b => b.Services)
                    );

                    user.Bookings = bookings.ToList();

                    return _profileMapper.MapToGetUserProfileWithBookings(user);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {Message}", ex.Message);
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
                    if (!await IsFreelancer(userId))
                        throw new UnauthorizedAccessException("User is not a freelancer.");

                    var userRepo = _unitOfWork.GetRepository<User>();
                    var bookingRepo = _unitOfWork.GetRepository<Booking>();

                    var user = await userRepo.FirstOrDefaultAsync(
                        predicate: u => u.Id == userId && u.IsActive,
                        include: q => q
                            .Include(u => u.Role)
                            .Include(u => u.Services)
                    );

                    if (user == null)
                        throw new NotFoundException("User not found.");

                    _profileMapper.UpdateFreelancerProfileFromRequest(request, user);
                    userRepo.UpdateAsync(user);

                    var bookings = await bookingRepo.GetListAsync(
                        predicate: b => b.FreelancerId == userId,
                        include: q => q
                            .Include(b => b.Customer)
                            .Include(b => b.Pets)
                            .Include(b => b.Services)
                    );

                    user.Bookings = bookings.ToList();

                    return _profileMapper.MapToGetFreelancerProfileWithBookings(user);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating freelancer profile: {Message}", ex.Message);
                throw;
            }
        }
    }
}
