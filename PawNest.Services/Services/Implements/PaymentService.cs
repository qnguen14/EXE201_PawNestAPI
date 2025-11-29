using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Context;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Payment;
using PawNest.Repository.Data.Responses.Payment;
using PawNest.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork<PawNestDbContext> _unitOfWork;
        private readonly Dictionary<PaymentMethod, IPaymentGateway> _paymentGateways;
        private readonly ILogger<PaymentService> _logger;
        private readonly decimal _commissionRate = 0.10m; // 10% commission

        public PaymentService(
            IUnitOfWork<PawNestDbContext> unitOfWork,
            VnPayGateway vnPayGateway,
            MoMoGateway moMoGateway,
            ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _paymentGateways = new Dictionary<PaymentMethod, IPaymentGateway>
            {
                { PaymentMethod.VNPay, vnPayGateway },
                { PaymentMethod.MoMo, moMoGateway }
            };
        }

        public async Task<PaymentGatewayResponse> CreatePaymentAsync(PaymentRequest request, string ipAddress)
        {
            try
            {
                // Get booking repository
                var bookingRepo = _unitOfWork.GetRepository<Booking>();

                // Get booking with related data
                var booking = await bookingRepo.FirstOrDefaultAsync(
                    predicate: b => b.BookingId == request.BookingId,
                    include: query => query
                        .Include(b => b.Customer)
                        .Include(b => b.Freelancer)
                );

                if (booking == null)
                {
                    _logger.LogWarning("Booking not found: {BookingId}", request.BookingId);
                    return new PaymentGatewayResponse
                    {
                        Success = false,
                        Message = "Booking not found"
                    };
                }

                // Check if booking is already paid
                if (booking.IsPaid)
                {
                    _logger.LogWarning("Booking already paid: {BookingId}", request.BookingId);
                    return new PaymentGatewayResponse
                    {
                        Success = false,
                        Message = "Booking has already been paid"
                    };
                }

                // Check booking status
                if (booking.Status == BookingStatus.Cancelled)
                {
                    _logger.LogWarning("Cannot pay for cancelled booking: {BookingId}", request.BookingId);
                    return new PaymentGatewayResponse
                    {
                        Success = false,
                        Message = "Cannot pay for cancelled booking"
                    };
                }

                // Check if payment already exists for this booking
                var paymentRepo = _unitOfWork.GetRepository<Payment>();
                var existingPayment = await paymentRepo.FirstOrDefaultAsync(
                    predicate: p => p.BookingId == request.BookingId &&
                                   (p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Success)
                );

                if (existingPayment != null)
                {
                    if (existingPayment.Status == PaymentStatus.Success)
                    {
                        return new PaymentGatewayResponse
                        {
                            Success = false,
                            Message = "Payment already completed for this booking"
                        };
                    }

                    // If pending, we can create new payment URL
                    _logger.LogInformation("Existing pending payment found, creating new payment URL");
                }

                // Calculate commission
                decimal commissionAmount = booking.TotalPrice * _commissionRate;

                // Create payment record
                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid(),
                    BookingId = booking.BookingId,
                    Amount = booking.TotalPrice,
                    CommissionAmount = commissionAmount,
                    Method = request.Method.ToString(),
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await paymentRepo.InsertAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Payment record created: {PaymentId} for Booking: {BookingId}",
                    payment.PaymentId, booking.BookingId);

                // Create payment URL using appropriate gateway
                var gateway = _paymentGateways[request.Method];
                var gatewayRequest = new PaymentGatewayRequest
                {
                    BookingId = booking.BookingId,
                    Amount = booking.TotalPrice,
                    OrderInfo = string.IsNullOrEmpty(request.Description)
                        ? $"Thanh toan booking #{booking.BookingId}"
                        : request.Description,
                    ReturnUrl = request.ReturnUrl ?? GetDefaultReturnUrl(request.Method),
                    NotifyUrl = GetDefaultNotifyUrl(request.Method),
                    IpAddress = ipAddress
                };

                var paymentUrlResponse = await gateway.CreatePaymentUrl(gatewayRequest);

                if (paymentUrlResponse.Success)
                {
                    _logger.LogInformation("Payment URL created successfully for Payment: {PaymentId}", payment.PaymentId);
                }
                else
                {
                    _logger.LogError("Failed to create payment URL for Payment: {PaymentId}. Error: {Error}",
                        payment.PaymentId, paymentUrlResponse.Message);
                }

                return paymentUrlResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for Booking: {BookingId}", request.BookingId);
                return new PaymentGatewayResponse
                {
                    Success = false,
                    Message = "An error occurred while creating payment"
                };
            }
        }

        public async Task<PaymentCallbackResponse> ProcessPaymentCallbackAsync(
            PaymentMethod method,
            Dictionary<string, string> queryParams)
        {
            try
            {
                var gateway = _paymentGateways[method];
                var callbackResponse = await gateway.ProcessCallback(queryParams);

                _logger.LogInformation("Payment callback processed. Method: {Method}, Success: {Success}, Message: {Message}",
                    method, callbackResponse.Success, callbackResponse.Message);

                return callbackResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback for method: {Method}", method);
                return new PaymentCallbackResponse
                {
                    Success = false,
                    Message = "Error processing payment callback"
                };
            }
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid bookingId, PaymentCallbackResponse callbackResponse)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var paymentRepo = _unitOfWork.GetRepository<Payment>();
                    var bookingRepo = _unitOfWork.GetRepository<Booking>();

                    // Get the most recent payment for this booking
                    var payment = await paymentRepo.FirstOrDefaultAsync(
                        predicate: p => p.BookingId == bookingId,
                        orderBy: query => query.OrderByDescending(p => p.CreatedAt)
                    );

                    if (payment == null)
                    {
                        _logger.LogWarning("Payment not found for Booking: {BookingId}", bookingId);
                        return false;
                    }

                    // Update payment status
                    payment.Status = callbackResponse.Status;
                    paymentRepo.UpdateAsync(payment);

                    _logger.LogInformation("Payment status updated: {PaymentId}, Status: {Status}",
                        payment.PaymentId, callbackResponse.Status);

                 
                    // If payment successful, update booking
                    if (callbackResponse.Success)
                    {
                        var booking = await bookingRepo.FirstOrDefaultAsync(
                            predicate: b => b.BookingId == bookingId
                        );

                        if (booking != null)
                        {
                            booking.IsPaid = true;
                            booking.Status = BookingStatus.Confirmed;
                            booking.UpdatedAt = DateTime.UtcNow;
                            bookingRepo.UpdateAsync(booking);

                            _logger.LogInformation("Booking confirmed after successful payment: {BookingId}", bookingId);
                        }
                    }

                    return true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status for Booking: {BookingId}", bookingId);
                return false;
            }
        }
        public async Task<PaymentStatus> CheckPaymentStatus(Guid bookingId)
        {
            var payment = await GetPaymentByBookingIdAsync(bookingId);
            return payment?.Status ?? PaymentStatus.Pending;
        }

        public async Task<Payment?> GetPaymentByBookingIdAsync(Guid bookingId)
        {
            try
            {
                var paymentRepo = _unitOfWork.GetRepository<Payment>();

                var payment = await paymentRepo.FirstOrDefaultAsync(
                    predicate: p => p.BookingId == bookingId,
                    orderBy: query => query.OrderByDescending(p => p.CreatedAt),
                    include: query => query.Include(p => p.Booking)
                );

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment for Booking: {BookingId}", bookingId);
                return null;
            }
        }

        public async Task<Payment?> GetPaymentByIdAsync(Guid paymentId)
        {
            try
            {
                var paymentRepo = _unitOfWork.GetRepository<Payment>();

                var payment = await paymentRepo.FirstOrDefaultAsync(
                    predicate: p => p.PaymentId == paymentId,
                    include: query => query.Include(p => p.Booking)
                );

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment: {PaymentId}", paymentId);
                return null;
            }
        }

        public async Task<bool> CancelPaymentAsync(Guid paymentId)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var paymentRepo = _unitOfWork.GetRepository<Payment>();

                    var payment = await paymentRepo.FirstOrDefaultAsync(
                        predicate: p => p.PaymentId == paymentId
                    );

                    if (payment == null)
                    {
                        _logger.LogWarning("Payment not found: {PaymentId}", paymentId);
                        return false;
                    }

                    if (payment.Status != PaymentStatus.Pending)
                    {
                        _logger.LogWarning("Cannot cancel payment with status: {Status}", payment.Status);
                        return false;
                    }

                    payment.Status = PaymentStatus.Cancelled;
                    paymentRepo.UpdateAsync(payment);

                    _logger.LogInformation("Payment cancelled: {PaymentId}", paymentId);
                    return true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment: {PaymentId}", paymentId);
                return false;
            }
        }

        private string GetDefaultReturnUrl(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.VNPay => "https://yoursite.com/payment/vnpay-callback",
                PaymentMethod.MoMo => "https://yoursite.com/payment/momo-callback",
                _ => "https://yoursite.com/payment/callback"
            };
        }
        private string GetDefaultNotifyUrl(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.VNPay => "https://your-backend.com/api/payment/vnpay-ipn",
                PaymentMethod.MoMo => "https://your-backend.com/api/payment/momo-ipn",
                _ => "https://your-backend.com/api/payment/ipn"
            };
        }
    }
}
