using PawNest.DAL.Data.Requests.Payment;
using PawNest.DAL.Data.Responses.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request);
        Task<bool> HandleProviderCallbackAsync(string provider, IDictionary<string, string> payload);
    }
}
