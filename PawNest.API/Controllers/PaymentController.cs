using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.API.Constants;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Requests.Payment;

namespace PawNest.API.Controllers
{

    [ApiController]
    [Route(ApiEndpointConstants.Payment.PaymentEndpoint)]
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
        /// Create payment for a booking
        /// POST /api/v1/payment/create
        /// </summary>
        [HttpPost("create")]
        [Authorize]
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
            var result = await _paymentService.CreatePaymentAsync(request, ipAddress);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }

            return Ok(new
            {
                success = true,
                paymentUrl = result.PaymentUrl,
                message = "Payment URL created successfully"
            });
        }

        /// <summary>
        /// VNPay callback endpoint
        /// GET /api/v1/payment/vnpay-callback
        /// </summary>
        [HttpGet("vnpay-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> VNPayCallback()
        {
            try
            {
                var queryParams = Request.Query.ToDictionary(
                    x => x.Key,
                    x => x.Value.ToString()
                );

                _logger.LogInformation("VNPay callback received with {Count} parameters", queryParams.Count);

                var callbackResponse = await _paymentService.ProcessPaymentCallbackAsync(
                    PaymentMethod.VNPay,
                    queryParams
                );

                if (callbackResponse.Success && queryParams.ContainsKey("vnp_TxnRef"))
                {
                    var bookingId = Guid.Parse(queryParams["vnp_TxnRef"]);
                    var updated = await _paymentService.UpdatePaymentStatusAsync(bookingId, callbackResponse);

                    if (updated)
                    {
                        _logger.LogInformation("Payment updated successfully for Booking: {BookingId}", bookingId);
                        return Redirect($"/payment-success?bookingId={bookingId}&transactionId={callbackResponse.TransactionId}");
                    }
                    else
                    {
                        _logger.LogWarning("Failed to update payment for Booking: {BookingId}", bookingId);
                    }
                }

                return Redirect($"/payment-failed?message={Uri.EscapeDataString(callbackResponse.Message)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback");
                return Redirect("/payment-failed?message=System+error");
            }
        }

        /// <summary>
        /// MoMo callback endpoint (IPN - Instant Payment Notification)
        /// POST /api/v1/payment/momo-callback
        /// </summary>
        [HttpPost("momo-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> MoMoCallback()
        {
            try
            {
                var formData = Request.Form.ToDictionary(
                    x => x.Key,
                    x => x.Value.ToString()
                );

                _logger.LogInformation("MoMo IPN callback received");

                var callbackResponse = await _paymentService.ProcessPaymentCallbackAsync(
                    PaymentMethod.MoMo,
                    formData
                );

                if (formData.ContainsKey("orderId"))
                {
                    var bookingId = Guid.Parse(formData["orderId"]);
                    await _paymentService.UpdatePaymentStatusAsync(bookingId, callbackResponse);
                }

                // MoMo expects this response format for IPN
                return Ok(new
                {
                    partnerCode = formData.GetValueOrDefault("partnerCode"),
                    orderId = formData.GetValueOrDefault("orderId"),
                    requestId = formData.GetValueOrDefault("requestId"),
                    resultCode = callbackResponse.Success ? 0 : 1,
                    message = callbackResponse.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MoMo callback");
                return Ok(new { resultCode = 1, message = "System error" });
            }
        }

        /// <summary>
        /// MoMo return URL endpoint (user redirect after payment)
        /// GET /api/v1/payment/momo-return
        /// </summary>
        [HttpGet("momo-return")]
        [AllowAnonymous]
        public async Task<IActionResult> MoMoReturn()
        {
            try
            {
                var queryParams = Request.Query.ToDictionary(
                    x => x.Key,
                    x => x.Value.ToString()
                );

                _logger.LogInformation("MoMo return URL accessed");

                if (queryParams.ContainsKey("orderId"))
                {
                    var bookingId = queryParams["orderId"];
                    var resultCode = queryParams.GetValueOrDefault("resultCode", "1");

                    if (resultCode == "0")
                    {
                        var transactionId = queryParams.GetValueOrDefault("transId", "");
                        return Redirect($"/payment-success?bookingId={bookingId}&transactionId={transactionId}");
                    }
                }

                var message = queryParams.GetValueOrDefault("message", "Payment failed");
                return Redirect($"/payment-failed?message={Uri.EscapeDataString(message)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MoMo return");
                return Redirect("/payment-failed?message=System+error");
            }
        }

        /// <summary>
        /// Get payment details by booking ID
        /// GET /api/v1/payment/booking/{bookingId}
        /// </summary>
        [HttpGet("booking/{bookingId}")]
        [Authorize]
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
        /// Get payment details by payment ID
        /// GET /api/v1/payment/{paymentId}
        /// </summary>
        [HttpGet("{paymentId}")]
        [Authorize]
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
        /// Cancel a pending payment
        /// POST /api/v1/payment/{paymentId}/cancel
        /// </summary>
        [HttpPost("{paymentId}/cancel")]
        [Authorize]
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
    }
}
