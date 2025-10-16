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
            .HasOne(p => p.Customer)
            .WithMany(u => u.Pets)
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Service)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Booking-Customer relationship
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Customer)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Booking-Freelancer relationship
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Freelancer)
            .WithMany()
            .HasForeignKey(b => b.FreelancerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure many-to-many relationship between Booking and Pet
        modelBuilder.Entity<Booking>()
            .HasMany(b => b.Pets)
            .WithMany(p => p.Bookings)
            .UsingEntity<Dictionary<string, object>>(
                "BookingPet",
                j => j
                    .HasOne<Pet>()
                    .WithMany()
                    .HasForeignKey("PetId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Booking>()
                    .WithMany()
                    .HasForeignKey("BookingId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("BookingId", "PetId");
                    j.ToTable("BookingPet");
                });

        // Configure Report-Staff relationship
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Staff)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.StaffId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Review-Booking relationship
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Booking)
            .WithMany()
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

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

        // Configure unique constraint on User email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configure enum conversion for BookingStatus
        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<int>();
    }
}