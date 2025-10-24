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
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("PawNestV1");
        base.OnModelCreating(modelBuilder);

        // ============ BlacklistedToken Configuration ============
        modelBuilder.Entity<BlacklistedToken>(entity =>
        {
            entity.HasKey(bt => bt.Id);
            entity.HasIndex(bt => bt.TokenHash).IsUnique();
            entity.HasIndex(bt => bt.ExpiresAt);
        });

        // ============ User Configuration ============
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Posts)
                .WithOne(p => p.Staff)
                .HasForeignKey(p => p.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============ Service Configuration ============
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(s => s.ServiceId);

            entity.Property(s => s.Type)
                .HasConversion<int>();

            entity.HasOne(s => s.Freelancer)
                .WithMany(u => u.Services)
                .HasForeignKey(s => s.FreelancerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============ Pet Configuration ============
        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasKey(p => p.PetId);

            entity.HasOne(p => p.Customer)
                .WithMany(u => u.Pets)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============ Booking Configuration ============
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.BookingId);

            entity.Property(b => b.Status)
                .HasConversion<int>();

            // Booking -> Customer
            entity.HasOne(b => b.Customer)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> Freelancer
            entity.HasOne(b => b.Freelancer)
                .WithMany()
                .HasForeignKey(b => b.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many: Booking <-> Pet
            entity.HasMany(b => b.Pets)
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

            // Many-to-Many: Booking <-> Service
            entity.HasMany(b => b.Services)
                .WithMany(s => s.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingService",
                    j => j
                        .HasOne<Service>()
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Booking>()
                        .WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("BookingId", "ServiceId");
                        j.ToTable("BookingService");
                    });
        });

        // ============ Payment Configuration ============
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.PaymentId);

            entity.Property(p => p.Status)
                .HasConversion<int>();

            entity.HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============ Report Configuration ============
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(r => r.ReportId);

            entity.HasOne(r => r.Staff)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============ Review Configuration ============
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Freelancer)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(r => r.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Customer)
                .WithMany(u => u.ReviewsWritten)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}