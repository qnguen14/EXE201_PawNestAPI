using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.User;
using PawNest.DAL.Data.Responses.User;

namespace PawNest.BLL.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<CreateUserResponse>> GetAll();
    Task<CreateUserResponse> GetById(Guid id);
    Task<CreateUserResponse> Create(CreateUserRequest request);
    Task<User> Update(User user);
    Task<User> Register(User user);
    Task<User> Login(string email, string password);
}