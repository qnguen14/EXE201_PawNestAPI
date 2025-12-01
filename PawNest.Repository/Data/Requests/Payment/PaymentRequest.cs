using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Requests.Payment
{
    public enum PaymentMethod
    {
        PayOS
    }
    
    public class PaymentRequest
    {
        [Required(ErrorMessage = "BookingId is required")]
        public Guid BookingId { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public PaymentMethod Method { get; set; }

        // Optional: user's note
        public string? Description { get; set; }
    }
}
