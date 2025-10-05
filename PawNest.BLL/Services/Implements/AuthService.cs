using AutoMapper;
using Everwell.BLL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Exceptions;
using PawNest.DAL.Data.Requests.Auth;
using PawNest.DAL.Data.Requests.User;
using PawNest.DAL.Data.Responses.Auth;
using PawNest.DAL.Data.Responses.User;
using PawNest.DAL.Repositories.Interfaces;
using System.Data.Entity;

namespace PawNest.BLL.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenProvider _tokenProvider;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public AuthService(
            IUnitOfWork<PawNestDbContext> unitOfWork,
            TokenProvider tokenProvider,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<AuthService> logger,
            ITokenService tokenService,
            IEmailService emailService,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
            _tokenService = tokenService;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    throw new NotFoundException("Email and password must be provided.");
                }

                Console.WriteLine($"Login attempt for: {request.Email}");

                // Fetch user from the database
                var user = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(
                        predicate: u => u.Email == request.Email && u.IsActive,
                        include: query => EntityFrameworkQueryableExtensions.Include(query, x => x.Role)
                    );

                if (user == null)
                {
                    return null;
                }

                Console.WriteLine($"User found: {user.Email}");

                // Verify password
                bool isPasswordValid = VerifyPassword(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    Console.WriteLine("Password verification failed");
                    return new LoginResponse { IsUnauthorized = true };
                }

                Console.WriteLine("Password verified successfully");

                // If password is valid but stored as plain text, upgrade it to BCrypt hash
                if (!IsBCryptHash(user.Password))
                {
                    Console.WriteLine($"Upgrading plain text password to BCrypt hash for user: {user.Email}");
                    try
                    {
                        await UpdatePasswordAsync(user.Id, request.Password);
                        Console.WriteLine("Password upgraded successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to upgrade password: {ex.Message}");
                        // Don't fail login if password upgrade fails
                    }
                }

                // Generate token
                var token = _tokenProvider.Create(user);

                Console.WriteLine($"Login successful for: {user.Email}");

                // Return response
                var response = new LoginResponse
                {
                    Token = token,
                    Email = user.Email,
                    Expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryInMinutes"]))
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                throw;
            }
        }

        private bool VerifyPassword(string password, string storedPassword)
        {
            try
            {
                Console.WriteLine($"Verifying password for stored password: {storedPassword?.Substring(0, Math.Min(10, storedPassword?.Length ?? 0))}...");

                // Check if the stored password is a BCrypt hash
                if (IsBCryptHash(storedPassword))
                {
                    Console.WriteLine("Stored password is BCrypt hash - using BCrypt.Verify");
                    return BCrypt.Net.BCrypt.Verify(password, storedPassword);
                }
                else
                {
                    Console.WriteLine("Stored password appears to be plain text - using direct comparison");
                    // It's likely plain text (old format), do direct comparison
                    return password == storedPassword;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password verification error: {ex.Message}");

                // Fallback to plain text comparison for safety
                return password == storedPassword;
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
                            include: query => EntityFrameworkQueryableExtensions.Include(query, x => x.Role)
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

        private bool IsBCryptHash(string password)
        {
            // BCrypt hashes start with $2a$, $2b$, $2y$, or $2x$ followed by cost
            return password != null &&
                   password.Length >= 60 &&
                   (password.StartsWith("$2a$") ||
                    password.StartsWith("$2b$") ||
                    password.StartsWith("$2y$") ||
                    password.StartsWith("$2x$"));
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(u => u.Email == request.Email, null, null);

                if (existingUser != null)
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Tài khoản với Email này đã tồn tại. Vui lòng sử dụng Email khác."
                    };
                }

                if (request.Role != "Customer" && request.Role != "Freelancer")
                {
                    return new RegisterResponse
                    {
                        Success = false,
                        Message = "Vai trò không hợp lệ. Vui lòng chọn 'Customer' hoặc 'Freelancer'."
                    };
                }

                // Create user with Customer role (default role for public registration)
                var createUserRequest = new CreateUserRequest
                {
                    Name = request.Name,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    Password = request.Password,
                    Role = request.Role // Default role for public registration
                };

                // Use the existing UserService to create the user
                var userResponse = await _userService.Create(createUserRequest);

                // Convert CreateUserResponse to GetUserResponse manually (safer approach)
                var getUserResponse = new CreateUserResponse
                {
                    Id = userResponse.Id,
                    Name = userResponse.Name,
                    Email = userResponse.Email,
                    PhoneNumber = userResponse.PhoneNumber,
                    Address = userResponse.Address,
                    Role = userResponse.Role,
                    AvatarUrl = null, // Default for new users
                    // IsActive = userResponse.IsActive
                };

                return new RegisterResponse
                {
                    Success = true,
                    Message = "User registered successfully. You can now login with your credentials.",
                    User = getUserResponse
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return new RegisterResponse
                {
                    Success = false,
                    Message = "An error occurred during registration. Please try again."
                };
            }
        }

        public async Task<LogoutResponse> Logout(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new LogoutResponse
                    {
                        Success = false,
                        Message = "Cần có token để đăng xuất."
                    };
                }

                // Remove "Bearer " prefix if present
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring(7);
                }

                // Blacklist the token
                var blacklisted = await _tokenService.BlacklistTokenAsync(token);

                if (blacklisted)
                {
                    return new LogoutResponse
                    {
                        Success = true,
                        Message = "Đăng xuất thành công."
                    };
                }
                else
                {
                    return new LogoutResponse
                    {
                        Success = false,
                        Message = "Đăng xuất không thành. Vui lòng thử lại."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi đăng xuất: {ex.Message}");
                return new LogoutResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi server."
                };
            }
        }

        public async Task<bool> SendPasswordResetCodeAsync(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmail(email);
                if (user == null)
                {
                    // Don't reveal if email exists or not for security
                    return true;
                }

                // Generate 6-digit code
                var resetCode = _tokenService.GeneratePasswordResetCode(user.Id);

                // Send email with code
                await _emailService.SendPasswordResetCodeAsync(user.Email, resetCode, user.Name);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending reset code: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerifyResetCodeAndResetPasswordAsync(string code, string email, string newPassword)
        {
            try
            {
                if (!_tokenService.ValidatePasswordResetCode(code, email, out Guid userId))
                    return false;

                var user = await _userService.GetById(userId);
                if (user == null)
                    return false;

                // Update user password
                await _userService.UpdatePasswordAsync(userId, newPassword);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting password: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DisableAccount(Guid userId, string email, string userName)
        {
            try
            {
                var user = await _unitOfWork.GetRepository<User>()
                    .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive && u.Name == userName, null, null);
                // Generate 6-digit code
                var disableCode = _tokenService.GeneratePasswordResetCode(userId);
                // Send email with code
                await _emailService.SendDisableAccountCodeAsync(email, disableCode, userName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disabling account: {ex.Message}");
                return false;
            }
        }

        public Task<DisableAccountResponse> VerifyDisableCode(Guid userId, string email, string verifyCode)
        {
            try
            {
                if (_tokenService.ValidatePasswordResetCode(verifyCode, email, out Guid verifiedUserId) && verifiedUserId == userId)
                {
                    return Task.FromResult(new DisableAccountResponse
                    {
                        IsDisabled = true,
                        Message = "Tài khoản đã bị vô hiệu hóa."
                    });
                }
                else
                {
                    return Task.FromResult(new DisableAccountResponse
                    {
                        IsDisabled = false,
                        Message = "Mã xác nhận không hợp lệ hoặc đã hết hạn."
                    });
                }

            } catch (Exception ex)
            {
                Console.WriteLine($"Error verifying disable code: {ex.Message}");
                return Task.FromResult(new DisableAccountResponse
                {
                    IsDisabled = false,
                    Message = "An error occurred while verifying the code."
                });
            }
        }
    }
}
