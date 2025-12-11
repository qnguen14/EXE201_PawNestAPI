using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using PawNest.API.Middleware;
using PawNest.Services.Hubs;

namespace PawNest.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UsePawNestPipeline(this WebApplication app, IHostEnvironment env)
        {
            // Required for Heroku reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PawNest.API v1");
                options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                options.RoutePrefix = string.Empty;
            });
            app.UseCors(options =>
            {
                options
                    .SetIsOriginAllowed(origin =>
                    {
                        // Allow any localhost with any port
                        if (origin.StartsWith("http://localhost", StringComparison.OrdinalIgnoreCase))
                            return true;
            
                        // Allow specific production URLs
                        return origin == "https://pawnsnest-fe.vercel.app";
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
            app.MapHub<NotificationHub>("/notificationHub");

            app.UseAuthentication();
            app.UseMiddleware<TokenBlacklistMiddleware>();
            app.UseAuthorization();

            app.MapControllers();
            return app;
        }
    }
}
