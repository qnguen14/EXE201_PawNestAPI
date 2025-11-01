using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Requests.Payment
{
    public class PaymentGatewayRequest
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
        public string ReturnUrl { get; set; }
        public string IpAddress { get; set; }
    }
}
