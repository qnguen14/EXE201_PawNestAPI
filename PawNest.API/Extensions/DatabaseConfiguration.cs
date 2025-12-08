using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PawNest.Repository.Data.Context;

namespace PawNest.API.Extensions
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PawNestDbContext>(options =>
            {
                // Try to get connection string from environment variable first (Heroku)
                var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                       ?? configuration.GetConnectionString("DefaultConnection");

                // Convert Heroku's postgres:// format to Npgsql format if needed
                if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres://"))
                {
                    var databaseUri = new Uri(connectionString);
                    var userInfo = databaseUri.UserInfo.Split(':');
                    
                    connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.AbsolutePath.Trim('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
                }

                options.UseNpgsql(connectionString,
                    npgsqlOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null);
                    });
            });

            return services;
        }
    }
}