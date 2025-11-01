using Microsoft.EntityFrameworkCore;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Context;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Payment;
using PawNest.DAL.Data.Responses.Payment;
using PawNest.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork<PawNestDbContext> _unitOfWork;
        private readonly IVnPayService _vnPayService;
        private readonly IMoMoService _moMoService;

        public PaymentService(
            IUnitOfWork<PawNestDbContext> unitOfWork,
            IVnPayService vnPayService,
            IMoMoService moMoService)
        {
            _unitOfWork = unitOfWork;
            _vnPayService = vnPayService;
            _moMoService = moMoService;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
        {
            var bookingRepo = _unitOfWork.GetRepository<Booking>();
            var paymentRepo = _unitOfWork.GetRepository<Payment>();

            var booking = await bookingRepo.FirstOrDefaultAsync(
                    predicate: b => b.BookingId == request.BookingId
            );

            if (booking == null) throw new InvalidOperationException("Booking not found");
            if (booking.IsPaid) throw new InvalidOperationException("Booking already paid");

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                BookingId = booking.BookingId,
                Amount = booking.TotalPrice,
                CommissionAmount = ComputeCommission(booking.TotalPrice),
                Method = request.Method.ToString(),
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await paymentRepo.InsertAsync(payment);
            await _unitOfWork.SaveChangesAsync(); // persist payment

            string paymentUrl = request.Method switch
            {
                PaymentMethod.VNPay => _vnPayService.CreatePaymentUrl(payment, request.ReturnUrl),
                PaymentMethod.MoMo => await _moMoService.CreatePaymentUrlAsync(payment, request.ReturnUrl),
                _ => throw new InvalidOperationException("Unsupported payment method")
            };

            return new PaymentResponse
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                Method = payment.Method,
                Status = payment.Status.ToString(),
                Amount = payment.Amount,
                CreatedAt = payment.CreatedAt,
                PaymentUrl = paymentUrl
            };
        }
        private decimal ComputeCommission(decimal total) => Math.Round(total * 0.10m, 2);
        public async Task<bool> HandleProviderCallbackAsync(string provider, IDictionary<string, string> payload)
        {
            // Validate signature & parse paymentId / order info
            if (string.Equals(provider, "vnpay", StringComparison.OrdinalIgnoreCase))
            {
                var valid = _vnPayService.ValidateCallback(payload, out Guid paymentId, out bool success);
                if (!valid) return false;
                return await UpdatePaymentAndBookingAsync(paymentId, success ? PaymentStatus.Success : PaymentStatus.Failed);
            }
            else if (string.Equals(provider, "momo", StringComparison.OrdinalIgnoreCase))
            {
                var valid = _moMoService.ValidateCallback(payload, out Guid paymentId, out bool success);
                if (!valid) return false;
                return await UpdatePaymentAndBookingAsync(paymentId, success ? PaymentStatus.Success : PaymentStatus.Failed);
            }

            return false;
        }

        private async Task<bool> UpdatePaymentAndBookingAsync(Guid paymentId, PaymentStatus newStatus)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var paymentRepo = _unitOfWork.GetRepository<Payment>();
                var bookingRepo = _unitOfWork.GetRepository<Booking>();

                // ✅ Lấy Payment theo Id
                var payment = await paymentRepo.FirstOrDefaultAsync(
                    predicate: p => p.PaymentId == paymentId
                );

                if (payment == null)
                    throw new InvalidOperationException("Payment not found");

                // ✅ Update Payment
                payment.Status = newStatus;
                // payment.UpdatedAt = DateTime.UtcNow; // nếu bạn có field này
                paymentRepo.UpdateAsync(payment);

                // ✅ Nếu Payment success → update Booking
                if (newStatus == PaymentStatus.Success)
                {
                    var booking = await bookingRepo.FirstOrDefaultAsync(
                        predicate: b => b.BookingId == payment.BookingId
                    );

                    if (booking != null)
                    {
                        booking.IsPaid = true;
                        booking.Status = BookingStatus.Confirmed;
                        booking.UpdatedAt = DateTime.UtcNow;

                        bookingRepo.UpdateAsync(booking);
                    }
                }

                // ✅ SaveChanges được thực hiện trong ExecuteInTransactionAsync
                return true;
            });
        }

    }
}
