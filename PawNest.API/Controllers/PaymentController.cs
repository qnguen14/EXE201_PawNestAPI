using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Requests.Payment;

namespace PawNest.API.Controllers
{

    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new payment request
        /// </summary>
        [HttpPost(ApiEndpointConstants.Payment.CreatePaymentEndpoint)]
        [Authorize(Roles = "Customer,Freelancer")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request data",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            _logger.LogInformation("Creating payment for booking: {BookingId}", request.BookingId);
            var result = await _paymentService.CreatePaymentAsync(request, ipAddress);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }
            _logger.LogInformation("Generated PayOS URL: {PaymentUrl}", result.PaymentUrl);

            return Ok(new
            {
                success = true,
                paymentUrl = result.PaymentUrl,
                message = "Payment URL created successfully"
            });
        }

        /// <summary>
        /// PayOS callback handler
        /// </summary>
        [HttpGet(ApiEndpointConstants.Payment.PayOSCallbackEndpoint)]
        public async Task<IActionResult> PayOsCallback()
        {
            try
            {
                if (!Request.Query.Any())
                {
                    _logger.LogWarning("No query parameters received in callback");
                    return BadRequest("No parameters received");
                }
                var queryParams = Request.Query.ToDictionary(
                    x => x.Key,
                    x => x.Value.ToString()
                );

                _logger.LogInformation("PayOS callback received with {Count} parameters", queryParams.Count);

                // Process the callback to get payment status from PayOS
                var callbackResponse = await _paymentService.ProcessPaymentCallbackAsync(
                    PaymentMethod.PayOS,
                    queryParams
                );

                if (callbackResponse.Success && !string.IsNullOrEmpty(callbackResponse.TransactionId))
                {
                    // Find payment by TransactionId
                    var payment = await _paymentService.GetPaymentByTransactionIdAsync(callbackResponse.TransactionId);
                    
                    if (payment != null)
                    {
                        // Update payment status
                        var updated = await _paymentService.UpdatePaymentStatusAsync(payment.BookingId, callbackResponse);
                        
                        if (updated)
                        {
                            _logger.LogInformation("Payment updated successfully. BookingId: {BookingId}, TransactionId: {TransactionId}", 
                                payment.BookingId, callbackResponse.TransactionId);
                            return Redirect($"http://localhost:5173/payment-success?bookingId={payment.BookingId}&transactionId={callbackResponse.TransactionId}");
                        }
                        else
                        {
                            _logger.LogWarning("Failed to update payment. BookingId: {BookingId}", payment.BookingId);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Payment not found for TransactionId: {TransactionId}", callbackResponse.TransactionId);
                    }
                }

                return Redirect($"http://localhost:5173/payment-failed?message={Uri.EscapeDataString(callbackResponse.Message)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayOS callback");
                return Redirect("http://localhost:5173/payment-failed?message=System+error");
            }
        }

        /// <summary>
        /// Get payment information by booking ID
        /// </summary>
        [HttpGet(ApiEndpointConstants.Payment.GetPaymentByBookingIdEndpoint)]
        [Authorize(Roles = "Customer,Freelancer,Admin")]
        public async Task<IActionResult> GetPaymentByBookingId(Guid bookingId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByBookingIdAsync(bookingId);

                if (payment == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Payment not found for this booking"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        paymentId = payment.PaymentId,
                        bookingId = payment.BookingId,
                        amount = payment.Amount,
                        commissionAmount = payment.CommissionAmount,
                        method = payment.Method,
                        status = payment.Status.ToString(),
                        createdAt = payment.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment for Booking: {BookingId}", bookingId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving payment"
                });
            }
        }

        /// <summary>
        /// Get payment information by payment ID
        /// </summary>
        [HttpGet(ApiEndpointConstants.Payment.GetPaymentByIdEndpoint)]
        [Authorize]
        [Authorize(Roles = "Customer,Freelancer,Admin")]

        public async Task<IActionResult> GetPaymentById(Guid paymentId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByIdAsync(paymentId);

                if (payment == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Payment not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        paymentId = payment.PaymentId,
                        bookingId = payment.BookingId,
                        amount = payment.Amount,
                        commissionAmount = payment.CommissionAmount,
                        method = payment.Method,
                        status = payment.Status.ToString(),
                        createdAt = payment.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment: {PaymentId}", paymentId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving payment"
                });
            }
        }

        /// <summary>
        /// Hủy thanh toán
        /// </summary>
        [HttpPost(ApiEndpointConstants.Payment.CancelPaymentEndpoint)]
        [Authorize(Roles = "Customer")]


        public async Task<IActionResult> CancelPayment(Guid paymentId)
        {
            try
            {
                var result = await _paymentService.CancelPaymentAsync(paymentId);

                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to cancel payment. Payment may not exist or is not in pending status."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Payment cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment: {PaymentId}", paymentId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while cancelling payment"
                });
            }
        }
        /// <summary>
        /// Kiểm tra trạng thái thanh toán theo booking ID
        /// </summary>
        [HttpGet(ApiEndpointConstants.Payment.CheckPaymentStatusEndpoint)]
        [Authorize(Roles = "Customer,Freelancer,Admin")]
        public async Task<IActionResult> CheckPaymentStatus([FromRoute] Guid bookingId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByBookingIdAsync(bookingId);

                if (payment == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No payment found for this booking",
                        status = "NotFound"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        paymentId = payment.PaymentId,
                        bookingId = payment.BookingId,
                        status = payment.Status.ToString(),
                        amount = payment.Amount,
                        method = payment.Method,
                        createdAt = payment.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment status for Booking: {BookingId}", bookingId);

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while checking payment status"
                });
            }
        }
    }
}
