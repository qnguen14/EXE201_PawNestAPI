using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PawNest.DAL.Data.Requests.Auth;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<RegisterResponse> Register(RegisterRequest request);
        //Task<LogoutResponse> Logout(string token);
        Task<bool> SendPasswordResetCodeAsync(string email);
        Task<bool> VerifyResetCodeAndResetPasswordAsync(string code, string email, string newPassword);
    }
}
