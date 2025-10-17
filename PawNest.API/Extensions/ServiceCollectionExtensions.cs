using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
