// using PawNest.DAL.Data.Entities;
using PawNest.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PawNest.DAL.Data.Exceptions;
using Microsoft.AspNetCore.Http;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Mappers;

namespace PawNest.BLL.Services
{
    public abstract class BaseService<T> where T : class
    {
        protected IUnitOfWork<PawNestDbContext> _unitOfWork;
        protected ILogger<T> _logger;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IMapperlyMapper _mapper;
        
        public BaseService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<T> logger, IHttpContextAccessor httpContextAccessor, IMapperlyMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        
        protected Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new UnauthorizedException("User ID not found in token");
            }
            return userId;
        }
        
        protected string GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedException("User email not found in token");
        }
        
        protected string GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new UnauthorizedException("User role not found in token");
        }
    }
}