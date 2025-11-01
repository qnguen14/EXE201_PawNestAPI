using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Entities
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        [Required]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("comment")]
        public string Comment { get; set; }

        [Column("response")]
        public string Response { get; set; }

        [Required]
        [Column("rating")]
        public double Rating { get; set; }

        [Required]
        [Column("freelancer_id")]
        [ForeignKey("Freelancer")]
        public Guid FreelancerId { get; set; }
        public virtual User Freelancer { get; set; }

        [Column("customer_id")]
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public virtual User Customer { get; set; }

        [Column("booking_id")]
        [ForeignKey("Booking")]
        public Guid BookingId { get; set; }
        public virtual Booking Booking { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("reponsed_at")]
        public DateTime? RepondedAt { get; set; }
    }
}
