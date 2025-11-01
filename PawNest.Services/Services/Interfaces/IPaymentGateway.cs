using PawNest.Repository.Data.Requests.Payment;
using PawNest.Repository.Data.Responses.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentGatewayResponse> CreatePaymentUrl(PaymentGatewayRequest request);
        Task<PaymentCallbackResponse> ProcessCallback(Dictionary<string, string> queryParams);
        Task<PaymentQueryResponse> QueryPayment(string transactionId);
    }
}
