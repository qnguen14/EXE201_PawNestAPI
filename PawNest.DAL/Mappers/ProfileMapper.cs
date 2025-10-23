using Riok.Mapperly.Abstractions;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Responses.Profile;

namespace PawNest.DAL.Mappers;

[Mapper]
public partial class ProfileMapper
{
    [MapProperty(nameof(User.Role.RoleName), nameof(GetUserProfile.Role))]
    public partial GetUserProfile MapToGetUserProfile(User user);
    
    // Handle null Role mapping
    private string MapRole(Role? role) => role?.RoleName ?? string.Empty;
}
