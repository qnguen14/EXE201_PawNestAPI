using Microsoft.Extensions.DependencyInjection;
using PawNest.Services.Services.Implements;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Repositories.Implements;
using PawNest.Repository.Repositories.Interfaces;
using PawNest.Repository.Data.Context;
using Everwell.Service.Infrastructure;
using PawNest.Repository.Mappers;

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
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<TokenProvider>();
            
            // Payment gateway
            services.AddScoped<PayOSGateway>();

            // Payment Service
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
