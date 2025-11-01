using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Responses.Payment
{
    public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public string Method { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentUrl { get; set; } = null!;
    }
}
