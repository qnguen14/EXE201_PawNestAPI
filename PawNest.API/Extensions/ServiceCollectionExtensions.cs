using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PawNest.DAL.Mappers;

namespace PawNest.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all core dependencies and configurations for the PawNest API.
        /// </summary>
        public static IServiceCollection AddPawNestCore(this IServiceCollection services, IConfiguration configuration)
        {
            // 🧱 Base setup
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSingleton<UserMapper>();
            services.AddSingleton<BookingMapper>();
            services.AddSingleton<PetMapper>();
            services.AddSingleton<PostMapper>();
            services.AddSingleton<ProfileMapper>();
            services.AddSingleton<ServiceMapper>();
            

            // 🌐 Modular configuration setup
            services.AddSwaggerDocumentation();
            services.AddJwtAuthentication(configuration);
            services.AddAuthorizationPolicies();
            services.AddDatabaseConfiguration(configuration);
            services.AddApplicationServices();

            return services;
        }
    }
}
