using Microsoft.Extensions.DependencyInjection;
using PawNest.BLL.Services.Implements;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Repositories.Implements;
using PawNest.DAL.Repositories.Interfaces;
using PawNest.DAL.Data.Context;
using Everwell.BLL.Infrastructure;
using PawNest.DAL.Mappers;

namespace PawNest.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork<PawNestDbContext>, UnitOfWork<PawNestDbContext>>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMapperlyMapper, MapperlyMapper>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPetService, PetService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IFreelancerService, FreelancerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<TokenProvider>();

            return services;
        }
    }
}
