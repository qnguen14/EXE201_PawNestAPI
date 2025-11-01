using PawNest.DAL.Data.Requests.Payment;
using PawNest.DAL.Data.Responses.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentGatewayResponse> CreatePaymentUrl(PaymentGatewayRequest request);
        Task<PaymentCallbackResponse> ProcessCallback(Dictionary<string, string> queryParams);
        Task<PaymentQueryResponse> QueryPayment(string transactionId);
    }
}
