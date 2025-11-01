using Microsoft.Extensions.Configuration;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Payment;
using PawNest.Repository.Data.Responses.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Implements
{
    public class MoMoGateway : IPaymentGateway
    {
        private readonly string _momoEndpoint;
        private readonly string _momoPartnerCode;
        private readonly string _momoAccessKey;
        private readonly string _momoSecretKey;
        private readonly HttpClient _httpClient;

        public MoMoGateway(IConfiguration configuration, HttpClient httpClient)
        {
            _momoEndpoint = configuration["MoMo:Endpoint"];
            _momoPartnerCode = configuration["MoMo:PartnerCode"];
            _momoAccessKey = configuration["MoMo:AccessKey"];
            _momoSecretKey = configuration["MoMo:SecretKey"];
            _httpClient = httpClient;
        }

        public async Task<PaymentGatewayResponse> CreatePaymentUrl(PaymentGatewayRequest request)
        {
            string orderId = request.BookingId.ToString();
            string requestId = Guid.NewGuid().ToString();
            long amount = (long)request.Amount;
            string orderInfo = request.OrderInfo;
            string returnUrl = request.ReturnUrl;
            string notifyUrl = request.ReturnUrl; // You might want a separate notify URL
            string extraData = "";

            // Create raw signature
            string rawSignature = $"accessKey={_momoAccessKey}" +
                                $"&amount={amount}" +
                                $"&extraData={extraData}" +
                                $"&ipnUrl={notifyUrl}" +
                                $"&orderId={orderId}" +
                                $"&orderInfo={orderInfo}" +
                                $"&partnerCode={_momoPartnerCode}" +
                                $"&redirectUrl={returnUrl}" +
                                $"&requestId={requestId}" +
                                $"&requestType=captureWallet";

            string signature = HmacSHA256(rawSignature, _momoSecretKey);

            var requestData = new
            {
                partnerCode = _momoPartnerCode,
                accessKey = _momoAccessKey,
                requestId = requestId,
                amount = amount,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                extraData = extraData,
                requestType = "captureWallet",
                signature = signature,
                lang = "vi"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(_momoEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var momoResponse = JsonSerializer.Deserialize<MoMoPaymentResponse>(responseString);

            if (momoResponse?.ResultCode == 0)
            {
                return new PaymentGatewayResponse
                {
                    Success = true,
                    PaymentUrl = momoResponse.PayUrl,
                    Message = "Success"
                };
            }

            return new PaymentGatewayResponse
            {
                Success = false,
                Message = momoResponse?.Message ?? "Unknown error"
            };
        }

        public Task<PaymentCallbackResponse> ProcessCallback(Dictionary<string, string> queryParams)
        {
            string orderId = queryParams["orderId"];
            string requestId = queryParams["requestId"];
            long amount = long.Parse(queryParams["amount"]);
            string orderInfo = queryParams["orderInfo"];
            string orderType = queryParams["orderType"];
            string transId = queryParams["transId"];
            int resultCode = int.Parse(queryParams["resultCode"]);
            string message = queryParams["message"];
            string payType = queryParams["payType"];
            string responseTime = queryParams["responseTime"];
            string extraData = queryParams["extraData"];
            string signature = queryParams["signature"];

            // Verify signature
            string rawSignature = $"accessKey={_momoAccessKey}" +
                                $"&amount={amount}" +
                                $"&extraData={extraData}" +
                                $"&message={message}" +
                                $"&orderId={orderId}" +
                                $"&orderInfo={orderInfo}" +
                                $"&orderType={orderType}" +
                                $"&partnerCode={_momoPartnerCode}" +
                                $"&payType={payType}" +
                                $"&requestId={requestId}" +
                                $"&responseTime={responseTime}" +
                                $"&resultCode={resultCode}" +
                                $"&transId={transId}";

            string computedSignature = HmacSHA256(rawSignature, _momoSecretKey);

            if (computedSignature != signature)
            {
                return Task.FromResult(new PaymentCallbackResponse
                {
                    Success = false,
                    Message = "Invalid signature"
                });
            }

            var status = resultCode == 0 ? PaymentStatus.Success : PaymentStatus.Failed;

            return Task.FromResult(new PaymentCallbackResponse
            {
                Success = resultCode == 0,
                TransactionId = transId,
                Amount = amount,
                Status = status,
                Message = message
            });
        }

        public async Task<PaymentQueryResponse> QueryPayment(string orderId)
        {
            string requestId = Guid.NewGuid().ToString();

            string rawSignature = $"accessKey={_momoAccessKey}" +
                                $"&orderId={orderId}" +
                                $"&partnerCode={_momoPartnerCode}" +
                                $"&requestId={requestId}";

            string signature = HmacSHA256(rawSignature, _momoSecretKey);

            var requestData = new
            {
                partnerCode = _momoPartnerCode,
                accessKey = _momoAccessKey,
                requestId = requestId,
                orderId = orderId,
                signature = signature,
                lang = "vi"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );

            var queryEndpoint = _momoEndpoint.Replace("/create", "/query");
            var response = await _httpClient.PostAsync(queryEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var momoResponse = JsonSerializer.Deserialize<MoMoQueryResponse>(responseString);

            if (momoResponse?.ResultCode == 0)
            {
                return new PaymentQueryResponse
                {
                    Success = true,
                    Status = PaymentStatus.Success,
                    Message = momoResponse.Message
                };
            }

            return new PaymentQueryResponse
            {
                Success = false,
                Status = PaymentStatus.Failed,
                Message = momoResponse?.Message ?? "Query failed"
            };
        }
        private string HmacSHA256(string data, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }
        private class MoMoPaymentResponse
        {
            public string PartnerCode { get; set; }
            public string RequestId { get; set; }
            public string OrderId { get; set; }
            public long Amount { get; set; }
            public long ResponseTime { get; set; }
            public string Message { get; set; }
            public int ResultCode { get; set; }
            public string PayUrl { get; set; }
        }

        private class MoMoQueryResponse
        {
            public string PartnerCode { get; set; }
            public string AccessKey { get; set; }
            public string RequestId { get; set; }
            public string OrderId { get; set; }
            public string Message { get; set; }
            public int ResultCode { get; set; }
            public long Amount { get; set; }
            public string TransId { get; set; }
        }

    }
}
