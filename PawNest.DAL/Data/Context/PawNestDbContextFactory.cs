using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PawNest.DAL.Data.Context;

public class PawNestDbContextFactory : IDesignTimeDbContextFactory<PawNestDbContext>
{
    public PawNestDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../PawNest.API");

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection") 
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var optionsBuilder = new DbContextOptionsBuilder<PawNestDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PawNestDbContext(optionsBuilder.Options);
    }
}