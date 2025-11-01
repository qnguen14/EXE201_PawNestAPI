using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Entities
{
    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed,
        Cancelled
    }

    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }

        public decimal Amount { get; set; }

        public decimal CommissionAmount { get; set; }

        public string Method { get; set; } = null!;

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        [ForeignKey("Booking")]
        public Guid BookingId { get; set; }
        public virtual Booking Booking { get; set; } = null!;
    }
}