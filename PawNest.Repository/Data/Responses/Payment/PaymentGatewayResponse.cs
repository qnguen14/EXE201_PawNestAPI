using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Repository.Data.Responses.Payment
{
    public class PaymentGatewayResponse
    {
        public bool Success { get; set; }
        public string PaymentUrl { get; set; }
        public string Message { get; set; }
    }
}
