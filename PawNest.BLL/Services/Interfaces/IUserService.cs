using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.User;
using PawNest.DAL.Data.Responses.User;

namespace PawNest.BLL.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<CreateUserResponse>> GetAll();
    Task<CreateUserResponse> GetById(Guid id);
    Task<CreateUserResponse> GetUserByEmail(string email);
    Task<CreateUserResponse> Create(CreateUserRequest request);
    Task<bool> UpdatePasswordAsync(Guid userId, string newPassword);
    Task<CreateUserResponse> Update(Guid userId, UpdateUserRequest request);

}