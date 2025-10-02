using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Entities
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public decimal CommissionAmount { get; set; }
        public string Method { get; set; } 
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // FK
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}
