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
                options.RoutePrefix = string.Empty; // Sets Swagger UI at the app's root
            });

            app.UseCors(options =>
            {
                options
                    .AllowAnyOrigin()
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
