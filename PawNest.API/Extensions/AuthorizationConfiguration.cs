using Microsoft.Extensions.DependencyInjection;

namespace PawNest.API.Extensions
{
    public static class AuthorizationConfiguration
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Administrator: full access
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("Admin"));

                // Staff: customer service, management control
                options.AddPolicy("RequireStaffRole", policy =>
                    policy.RequireRole("Staff"));

                // Freelancer: can manage own services, bookings
                options.AddPolicy("RequireFreelancerRole", policy =>
                    policy.RequireRole("Freelancer"));

                // Customer: basic access to pet-related services
                options.AddPolicy("RequireCustomerRole", policy =>
                    policy.RequireRole("Customer"));

                // Example of a combined policy (optional)
                options.AddPolicy("RequireStaffOrAdmin", policy =>
                    policy.RequireRole("Staff", "Admin"));
            });

            return services;
        }
    }
}
