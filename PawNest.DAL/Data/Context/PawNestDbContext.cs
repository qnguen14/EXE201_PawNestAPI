using Microsoft.EntityFrameworkCore;
using PawNest.DAL.Data.Entities;

namespace PawNest.DAL.Data.Context;

public class PawNestDbContext : DbContext
{
    public PawNestDbContext(DbContextOptions<PawNestDbContext> options) : base(options) { }
    public PawNestDbContext() { }

    public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Review> Reviews { get; set; }

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
            .HasForeignKey(p => p.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Service>()
            .HasOne(s => s.Freelancer)
            .WithMany(u => u.Services)
            .HasForeignKey(s => s.FreelancerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pet>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.Pets)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Service)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Pet)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Owner)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Disambiguate the two Review->User relationships
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Freelancer)
            .WithMany(u => u.ReviewsReceived)
            .HasForeignKey(r => r.FreelancerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Customer)
            .WithMany(u => u.ReviewsWritten)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Example index
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}