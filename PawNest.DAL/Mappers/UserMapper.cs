using Riok.Mapperly.Abstractions;
using PawNest.DAL.Data.Responses.User;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.User;

namespace PawNest.DAL.Mappers;

[Mapper]
public partial class UserMapper
{
    // CreateUserRequest to User
    [MapProperty(nameof(CreateUserRequest.Email), nameof(User.Email))]
    [MapProperty(nameof(CreateUserRequest.PhoneNumber), nameof(User.PhoneNumber))]
    [MapProperty(nameof(CreateUserRequest.Address), nameof(User.Address))]
    public partial User MapToUser(CreateUserRequest request);
    
    // User to CreateUserResponse
    [MapProperty(nameof(User.Role.RoleName), nameof(CreateUserResponse.Role))]
    public partial CreateUserResponse MapToCreateUserResponse(User user);
    
    // User to GetFreelancerResponse
    [MapProperty(nameof(User.Role.RoleName), nameof(GetFreelancerResponse.Role))]
    public partial GetFreelancerResponse MapToGetFreelancerResponse(User user);
    
    // Handle null Role mapping
    private string MapRole(Role? role) => role?.RoleName ?? string.Empty;
}