using Microsoft.EntityFrameworkCore;
using PawNest.DAL.Data.Entities;

namespace PawNest.DAL.Data.Context;

public class PawNestDbContext : DbContext
{
    public PawNestDbContext(DbContextOptions<PawNestDbContext> options)
        : base(options)
    {
    }
    
    public PawNestDbContext() { }

    public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("PawNestV1");
        base.OnModelCreating(modelBuilder);

        // Configure blacklisted tokens for logout functionality
        modelBuilder.Entity<BlacklistedToken>(entity =>
        {
            entity.HasKey(bt => bt.Id);
            entity.HasIndex(bt => bt.TokenHash).IsUnique();
            entity.HasIndex(bt => bt.ExpiresAt);
        });

        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.Staff)
            .HasForeignKey(p => p.StaffId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);

        base.OnModelCreating(modelBuilder);
    }
}