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

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("PawNestV1");
        base.OnModelCreating(modelBuilder);
        
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