using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.User;
using PawNest.Repository.Data.Responses.User;

namespace PawNest.Services.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<CreateUserResponse>> GetAll();
    Task<CreateUserResponse> GetById(Guid id);
    Task<CreateUserResponse> GetUserByEmail(string email);
    Task<CreateUserResponse> Create(CreateUserRequest request);
    Task<bool> UpdatePasswordAsync(Guid userId, string newPassword);
    Task<CreateUserResponse> Update(Guid userId, UpdateUserRequest request);
    Task<IEnumerable<CreateUserResponse>> GetUsersByRole(string roleName);
}