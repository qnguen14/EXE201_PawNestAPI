using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.DAL.Data.Requests.Auth;
using PawNest.DAL.Data.Responses.Auth;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<RegisterResponse> Register(RegisterRequest request);
        Task<LogoutResponse> Logout(string token);
        Task<bool> DisableAccount(Guid userId, string email, string userName);
        Task<DisableAccountResponse> VerifyDisableCode(Guid userId, string email, string verifyCode);
        Task<bool> SendPasswordResetCodeAsync(string email);
        Task<bool> VerifyResetCodeAndResetPasswordAsync(string code, string email, string newPassword);
    }
}
