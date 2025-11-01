using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Requests.Payment
{
    public enum PaymentMethod
    {
        VNPay,
        MoMo
    }
    public class PaymentRequest
    {
        [Required(ErrorMessage = "BookingId is required")]
        public Guid BookingId { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public PaymentMethod Method { get; set; }

        // Optional: dùng cho MoMo/VNPay trả về URL
        public string? ReturnUrl { get; set; }

        // Optional: trường note của user
        public string? Description { get; set; }
    }
}
