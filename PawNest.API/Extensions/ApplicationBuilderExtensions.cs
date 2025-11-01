using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using PawNest.API.Middleware;

namespace PawNest.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UsePawNestPipeline(this WebApplication app, IHostEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PawNest.API v1");
                    options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
                    options.RoutePrefix = string.Empty; // Sets Swagger UI at the app's root
                });
            }

            app.UseCors(options =>
            {
                options.SetIsOriginAllowed(origin => origin.StartsWith("http://localhost:"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });



            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware<TokenBlacklistMiddleware>();
            app.UseAuthorization();

            app.MapControllers();
            return app;
        }
    }
}
