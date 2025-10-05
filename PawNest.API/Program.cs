using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PawNest.BLL.Services.Implements;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Repositories.Implements;
using PawNest.DAL.Repositories.Interfaces;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignore circular references
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // Ignore null values (optional)
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT Bearer authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PawNest.API",
        Version = "v1",
        Description = "A Freelancer project - Pet Care Service API",
    });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token:"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         {
             new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = JwtBearerDefaults.AuthenticationScheme
                 }
             },
             new List<string>()
         }
     });
});

// Configure JWT Bearer authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
    options.RequireHttpsMetadata = true; // Enforce HTTPS in production for security
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate all critical JWT components
        ValidateIssuer = true, // Ensure token comes from trusted issuer
        ValidateAudience = true, // Ensure token is intended for this API
        ValidateLifetime = true, // Check token expiration
        ValidateIssuerSigningKey = true, // Verify token signature

        // JWT configuration from appsettings
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // Security settings
        ClockSkew = TimeSpan.Zero, // Disable default 5-minute clock skew for precise expiration
        NameClaimType = JwtRegisteredClaimNames.Sub, // Map user ID claim
        RoleClaimType = ClaimTypes.Role, // Map role-based authorization claim
    };
        //
        //         // JWT event handlers for debugging and logging
        //         options.Events = new JwtBearerEvents
        //         {
        //             OnMessageReceived = context =>
        //             {
        //                 // Log received tokens for debugging (remove in production)
        //                 var token = context
        //                     .Request.Headers["Authorization"]
        //                     .ToString()
        //                     .Replace("Bearer ", "");
        //                 Console.WriteLine($"Received token: {token}");
        //                 return Task.CompletedTask;
        //             },
        //         };
    });

// Configure role-based authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin")); // Full system access
    options.AddPolicy("RequireManagerRole", policy =>
        policy.RequireRole("Manager")); // Management operations
    options.AddPolicy("RequireConsultant", policy =>
        policy.RequireRole("Consultant")); // Healthcare provider access
    options.AddPolicy("RequireStaffRole", policy =>
        policy.RequireRole("Staff")); // Staff operations (test results, content)
    options.AddPolicy("RequireCustomerRole", policy =>
        policy.RequireRole("Customer")); // Patient/customer access
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure Unit of Work pattern for database transactions
builder.Services.AddScoped<IUnitOfWork<PawNestDbContext>,UnitOfWork<PawNestDbContext>>();
// Generic repository for common CRUD operations
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// HTTP context access for getting current user information
builder.Services.AddHttpContextAccessor();
// Configure AutoMapper with mapping profiles
// Register application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IFreelancerService, FreelancerService>();
// Configure 

// Configure database configuration

builder.Services.AddDbContext<PawNestDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        });
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    // Enable Swagger API documentation in development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PawNest.API v1"));
}

if (app.Environment.IsDevelopment())
{
    // Enable Swagger API documentation in development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v2.5/swagger.json", "PawNest.API v2.5"));
}

app.UseCors(options =>
{
    if (app.Environment.IsDevelopment())
    {
        // Development: Allow localhost and Daily.co video meeting domains
        options.SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost:"))// Local development servers
            .AllowAnyMethod() // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
            .AllowAnyHeader() // Allow all headers
            .AllowCredentials(); // Allow cookies and authentication headers
    }
});

app.UseHttpsRedirection();

app.UseAuthentication(); // Validate JWT tokens
                         // app.UseMiddleware<TokenBlacklistMiddleware>(); // Check for blacklisted tokens (logout functionality)
app.UseAuthorization(); // Apply role-based access control

// Map API controllers to handle HTTP requests
app.MapControllers();


app.Run();
