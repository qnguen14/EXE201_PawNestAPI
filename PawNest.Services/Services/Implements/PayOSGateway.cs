using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Payment;
using PawNest.Repository.Data.Responses.Payment;
using PawNest.Services.Services.Interfaces;

namespace PawNest.Services.Services.Implements
{
    /// <summary>
    /// PayOS payment gateway implementation
    /// </summary>
    public class PayOSGateway : IPaymentGateway
    {
        private readonly PayOS _payOS;
        private readonly ILogger<PayOSGateway> _logger;
        private readonly string _returnUrl;
        private readonly string _cancelUrl;

        public PayOSGateway(IConfiguration configuration, ILogger<PayOSGateway> logger)
        {
            _logger = logger;
            
            var clientId = configuration["PayOS:ClientId"];
            var apiKey = configuration["PayOS:ApiKey"];
            var checksumKey = configuration["PayOS:ChecksumKey"];
            _returnUrl = configuration["PayOS:ReturnUrl"] ?? "http://localhost:5173/payment-success";
            _cancelUrl = configuration["PayOS:CancelUrl"] ?? "http://localhost:5173/payment-failed";

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(checksumKey))
            {
                throw new ArgumentException("PayOS configuration is missing or incomplete");
            }

            _payOS = new PayOS(clientId, apiKey, checksumKey);
        }

        public async Task<PaymentGatewayResponse> CreatePaymentUrl(PaymentGatewayRequest request)
        {
            try
            {
                // Generate unique order code (PayOS requires long type)
                var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var items = new List<ItemData>
                {
                    new ItemData(
                        name: request.OrderInfo,
                        quantity: 1,
                        price: (int)request.Amount
                    )
                };

                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: (int)request.Amount,
                    description: request.OrderInfo,
                    items: items,
                    returnUrl: _returnUrl,
                    cancelUrl: _cancelUrl
                );

                var createPaymentResult = await _payOS.createPaymentLink(paymentData);

                _logger.LogInformation("PayOS payment link created successfully. OrderCode: {OrderCode}, CheckoutUrl: {CheckoutUrl}",
                    orderCode, createPaymentResult.checkoutUrl);

                return new PaymentGatewayResponse
                {
                    Success = true,
                    PaymentUrl = createPaymentResult.checkoutUrl,
                    TransactionId = orderCode.ToString(),
                    Message = "Payment URL created successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayOS payment link");
                return new PaymentGatewayResponse
                {
                    Success = false,
                    Message = $"Error creating payment: {ex.Message}"
                };
            }
        }

        public async Task<PaymentCallbackResponse> ProcessCallback(Dictionary<string, string> queryParams)
        {
            try
            {
                // PayOS uses webhook for payment confirmation
                // This method processes the return URL parameters
                
                if (!queryParams.TryGetValue("orderCode", out var orderCodeStr))
                {
                    return new PaymentCallbackResponse
                    {
                        Success = false,
                        Status = PaymentStatus.Failed,
                        Message = "Missing orderCode parameter"
                    };
                }

                if (!long.TryParse(orderCodeStr, out var orderCode))
                {
                    return new PaymentCallbackResponse
                    {
                        Success = false,
                        Status = PaymentStatus.Failed,
                        Message = "Invalid orderCode format"
                    };
                }

                // Query payment status from PayOS
                var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);

                var status = paymentInfo.status switch
                {
                    "PAID" => PaymentStatus.Success,
                    "CANCELLED" => PaymentStatus.Cancelled,
                    "PENDING" => PaymentStatus.Pending,
                    _ => PaymentStatus.Failed
                };

                var success = paymentInfo.status == "PAID";

                _logger.LogInformation("PayOS callback processed. OrderCode: {OrderCode}, Status: {Status}",
                    orderCode, paymentInfo.status);

                return new PaymentCallbackResponse
                {
                    Success = success,
                    Status = status,
                    TransactionId = orderCode.ToString(),
                    Amount = paymentInfo.amountPaid,
                    Message = success ? "Payment successful" : $"Payment {paymentInfo.status.ToLower()}",
                    PaymentDate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayOS callback");
                return new PaymentCallbackResponse
                {
                    Success = false,
                    Status = PaymentStatus.Failed,
                    Message = $"Error processing callback: {ex.Message}"
                };
            }
        }

        public async Task<PaymentQueryResponse> QueryPayment(string transactionId)
        {
            try
            {
                if (!long.TryParse(transactionId, out var orderCode))
                {
                    return new PaymentQueryResponse
                    {
                        Success = false,
                        Message = "Invalid transaction ID format"
                    };
                }

                var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);

                var status = paymentInfo.status switch
                {
                    "PAID" => PaymentStatus.Success,
                    "CANCELLED" => PaymentStatus.Cancelled,
                    "PENDING" => PaymentStatus.Pending,
                    _ => PaymentStatus.Failed
                };

                return new PaymentQueryResponse
                {
                    Success = true,
                    Status = status,
                    TransactionId = transactionId,
                    Amount = paymentInfo.amountPaid,
                    Message = $"Payment status: {paymentInfo.status}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying PayOS payment: {TransactionId}", transactionId);
                return new PaymentQueryResponse
                {
                    Success = false,
                    Message = $"Error querying payment: {ex.Message}"
                };
            }
        }
    }
}

