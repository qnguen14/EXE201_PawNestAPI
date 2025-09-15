using PawNest.DAL.Data.Responses.User;
using PawNest.DAL.Data.Entities;

namespace PawNest.BLL.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<CreateUserResponse>> GetAll();
    Task<CreateUserResponse> GetById(Guid id);
    Task<User> Create(User user);
    Task<User> Update(User user);
    Task<User> Register(User user);
    Task<User> Login(string email, string password);
}