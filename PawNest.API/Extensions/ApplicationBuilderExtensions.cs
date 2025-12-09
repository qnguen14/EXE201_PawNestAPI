using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using PawNest.API.Middleware;

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

            // Swagger should be allowed in Production on Heroku
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PawNest.API v1");
                options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                options.RoutePrefix = string.Empty;
            });

            // -------------------------
            // FIXED CORS (NO CRASH)
            // -------------------------

            // Add your real frontend URLs here:
            var allowedOrigins = new[]
            {
                "http://localhost",
                "https://pawnsnest-fe.vercel.app", // example
            };

            app.UseCors(options =>
            {
                options
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            // Do NOT use HTTPS redirect on Heroku
            // Heroku handles HTTPS externally
            // app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseMiddleware<TokenBlacklistMiddleware>();
            app.UseAuthorization();

            app.MapControllers();
            return app;
        }
    }
}
