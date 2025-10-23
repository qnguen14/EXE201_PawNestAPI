using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Requests.User;
using PawNest.DAL.Data.Responses.User;
using PawNest.DAL.Repositories.Interfaces;
using System.Data;

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

    public async Task<CreateUserResponse> GetUserByEmail(string email)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(
                    predicate: u => u.Email == email && u.IsActive,
                    include: u => u.Include(x => x.Role)
                );
            if (user == null)
            {
                throw new NotFoundException("User with email " + email + " not found.");
            }
            return _mapper.Map<CreateUserResponse>(user);
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

    public async Task<CreateUserResponse> Create(CreateUserRequest request)
    {
        try
        {
            // Validate the user entity (e.g., check for existing email)
            var existingUser = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(
                    predicate: u => u.Email == request.Email && u.IsActive);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {request.Email} already exists.");
            }

            // Resolve and set the role (Role is a separate entity)
            var role = await _unitOfWork.GetRepository<Role>()
                .FirstOrDefaultAsync(predicate: r => r.RoleName == request.Role);
            if (role == null)
            {
                throw new NotFoundException($"Role '{request.Role}' not found.");
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request cannot be null.");
                }

                // Map the basic fields
                var newUser = _mapper.Map<User>(request);

                // 1. Generate a new ID
                newUser.Id = Guid.NewGuid();

                // 2. Hash the password
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

                newUser.RoleId = role.Id;       // set FK
                newUser.Role = role;            // optional: set navigation

                // 4. Set default values
                newUser.IsActive = true;

                // 5. Validate required fields
                if (string.IsNullOrEmpty(newUser.Name))
                    throw new ArgumentException("Name is required");
                if (string.IsNullOrEmpty(newUser.Email))
                    throw new ArgumentException("Email is required");
                if (string.IsNullOrEmpty(newUser.PhoneNumber))
                    throw new ArgumentException("Phone number is required");
                if (string.IsNullOrEmpty(newUser.Address))
                    throw new ArgumentException("Address is required");

                Console.WriteLine($"Creating user: {newUser.Email} with role: {role.RoleName}");

                // Add the new user
                await _unitOfWork.GetRepository<User>().InsertAsync(newUser);

                return _mapper.Map<CreateUserResponse>(newUser);
            });

        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while creating a user: " + ex.Message);
            throw;
        }
    }

    public async Task<bool> UpdatePasswordAsync(Guid userId, string newPassword)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingUser = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(
                        predicate: u => u.Id == userId && u.IsActive,
                        include: u => u.Include(x => x.Role)
                    );

                if (existingUser == null)
                {
                    throw new InvalidOperationException("User not found.");
                }

                // Hash the new password before saving
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

                _unitOfWork.GetRepository<User>().UpdateAsync(existingUser);

                return true;
            });
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<CreateUserResponse> Update(Guid userId, UpdateUserRequest request)
    {
        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingUser = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(
                        predicate: u => u.Id == userId && u.IsActive,
                        include: u => u.Include(x => x.Role)
                    );
                if (existingUser == null)
                {
                    throw new InvalidOperationException("User not found.");
                }
                // Update fields if provided
                if (!string.IsNullOrEmpty(request.Name))
                    existingUser.Name = request.Name;
                if (!string.IsNullOrEmpty(request.PhoneNumber))
                    existingUser.PhoneNumber = request.PhoneNumber;
                if (!string.IsNullOrEmpty(request.Address))
                    existingUser.Address = request.Address;
                // If role is being updated, resolve the new role
                if (!string.IsNullOrEmpty(request.Role))
                {
                    var role = await _unitOfWork.GetRepository<Role>()
                        .FirstOrDefaultAsync(predicate: r => r.RoleName == request.Role);
                    if (role == null)
                    {
                        throw new NotFoundException($"Role '{request.Role}' not found.");
                    }
                    existingUser.RoleId = role.Id;   // set FK
                    existingUser.Role = role;        // optional: set navigation
                }
                _unitOfWork.GetRepository<User>().UpdateAsync(existingUser);
                return _mapper.Map<CreateUserResponse>(existingUser);
            });

        } catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<GetUserReponse> GetCurrentUserProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.GetRepository<User>()
                .FirstOrDefaultAsync(
                    predicate: u => u.Id == userId && u.IsActive,
                    include: u => u
                        .Include(x => x.Role)
                        .Include(x => x.Pets)
                        .Include(x => x.Bookings)
                        .ThenInclude(b => b.Pets)
                );
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            return _mapper.Map<GetUserReponse>(user);

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}