using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawNest.Repository.Data.Exceptions;
using PawNest.Repository.Data.Responses.Profile;
using PawNest.Repository.Mappers;

namespace PawNest.Services.Services.Implements
{
    public class ProfileService : BaseService<ProfileService>, IProfileService
    {
        private readonly IUnitOfWork<PawNestDbContext> _unitOfWork;
        private readonly IMapperlyMapper _profileMapper;
        private readonly ILogger<ProfileService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<ProfileService> logger, IMapperlyMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _profileMapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<GetUserProfile> GetProfileAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                var userRepo = _unitOfWork.GetRepository<User>();

                var user = await userRepo.FirstOrDefaultAsync(
                    predicate: u => u.Id == userId && u.IsActive,
                    orderBy: null,
                    include: q => q.Include(u => u.Role)
                );

                if (user == null)
                {
                    throw new NotFoundException("User not found.");
                }
                
                return _profileMapper.MapToGetUserProfile(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
