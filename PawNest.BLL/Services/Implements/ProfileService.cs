using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Profile;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PawNest.BLL.Services.Implements
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork<PawNestDbContext> _unitOfWork;

        public ProfileService(IUnitOfWork<PawNestDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            var user = await userRepo.FirstOrDefaultAsync(
                predicate: u => u.Id == userId,
                orderBy: null,
                include: q => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(q, u => u.Role)
            );


            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                RoleName = user.Role?.RoleName ?? "User",
                IsActive = user.IsActive
            };
        }
    }
}
