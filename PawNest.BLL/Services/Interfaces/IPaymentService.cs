using PawNest.DAL.Data.Entities;
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
        Task<PaymentGatewayResponse> CreatePaymentAsync(PaymentRequest request, string ipAddress);
        Task<PaymentCallbackResponse> ProcessPaymentCallbackAsync(PaymentMethod method, Dictionary<string, string> queryParams);
        Task<bool> UpdatePaymentStatusAsync(Guid bookingId, PaymentCallbackResponse callbackResponse);
        Task<Payment?> GetPaymentByBookingIdAsync(Guid bookingId);
        Task<Payment?> GetPaymentByIdAsync(Guid paymentId);
        Task<bool> CancelPaymentAsync(Guid paymentId);
    }
}
