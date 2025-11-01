using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.DAL.Data.Responses.Payment
{
    public class PaymentQueryResponse
    {
        public bool Success { get; set; }
        public PaymentStatus Status { get; set; }
        public string Message { get; set; }
    }
}
