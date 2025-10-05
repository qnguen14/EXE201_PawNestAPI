using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PawNest.DAL.Data.Entities;

[Table("Users")]
public class User
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Column("email")]
    [StringLength(256)]
    public string Email { get; set; }

    [Required]
    [Column("phone_number")]
    [StringLength(10)]
    public string PhoneNumber { get; set; }

    [Required]
    [Column("address")]
    public string Address { get; set; }

    [Required]
    [Column("password")]
    public string Password { get; set; }

    [Required]
    [Column("role_id")]
    public int RoleId { get; set; }
    public virtual Role Role { get; set; }

    [Column("avatar_url")]
    [StringLength(1000)]
    public string? AvatarUrl { get; set; } = null;

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    // public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
    public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
}