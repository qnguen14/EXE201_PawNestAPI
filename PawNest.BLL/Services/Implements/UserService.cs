using AutoMapper;
using PawNest.BLL.Services;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Responses.User;
using PawNest.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;

namespace PawNest.BLL.Services.Implements;

public class UserService : BaseService<UserService>, IUserService
{
    public UserService(IUnitOfWork<PawNestDbContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CreateUserResponse>> GetAll()
    {
        try
        {
            var users = await _unitOfWork.GetRepository<User>()
                .GetListAsync(
                    predicate: null, // Get all users (both active and inactive)
                    include: u => u.Include(x => x.Role),
                    orderBy: u => u.OrderBy(n => n.Name)
                );

            

            if (users == null || !users.Any())
            {
                throw new NotFoundException("No users found.");
            }

            return _mapper.Map<IEnumerable<CreateUserResponse>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving users: " + ex.Message);
            throw;
        }
    }

    public async Task<CreateUserResponse> GetById(Guid id)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(
                    predicate: null, // Get all users (both active and inactive)
                    include: u => u.Include(x => x.Role)
                );

            if (user == null)
            {
                throw new NotFoundException("User with ID " + id + " not found.");
            }

            return _mapper.Map<CreateUserResponse>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving users: " + ex.Message);
            throw;
        }
    }

    public Task<User> Create(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> Update(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> Register(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> Login(string email, string password)
    {
        throw new NotImplementedException();
    }
}