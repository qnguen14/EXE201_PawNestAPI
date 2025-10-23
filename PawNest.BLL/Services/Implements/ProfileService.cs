using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PawNest.DAL.Data.Responses.Profile;

namespace PawNest.BLL.Services.Implements
{
    public class ProfileService : BaseService<ProfileService>, IProfileService
    {
        private readonly IUnitOfWork<PawNestDbContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProfileService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<ProfileService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
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
                    include: q => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(q, u => u.Role)
                );


                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                var result = _mapper.Map<GetUserProfile>(user);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
