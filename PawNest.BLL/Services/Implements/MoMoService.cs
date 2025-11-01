using Microsoft.Extensions.Options;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PawNest.BLL.Services.Implements
{
    public class MoMoOptions
    {
        public string Endpoint { get; set; } = null!; // e.g. https://test-payment.momo.vn/v2/gateway/api/create
        public string PartnerCode { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public string NotifyUrl { get; set; } = null!;
    }
    public class MoMoService : IMoMoService
    {
        private readonly MoMoOptions _opts;
        private readonly HttpClient _http;

        public MoMoService(IOptions<MoMoOptions> opts, HttpClient http)
        {
            _opts = opts.Value;
            _http = http;
        }

        public async Task<string> CreatePaymentUrlAsync(Payment payment, string? returnUrl = null)
        {
            var orderId = payment.PaymentId.ToString();
            var requestId = Guid.NewGuid().ToString();
            var amount = ((long)(payment.Amount * 100)).ToString(); // if MoMo expects cents; adjust as needed

            var rawData = new StringBuilder();
            rawData.Append($"accessKey={_opts.AccessKey}");
            rawData.Append($"&amount={amount}");
            rawData.Append($"&extraData=");
            rawData.Append($"&ipnUrl={_opts.NotifyUrl}");
            rawData.Append($"&orderId={orderId}");
            rawData.Append($"&orderInfo=Payment for booking {payment.BookingId}");
            rawData.Append($"&partnerCode={_opts.PartnerCode}");
            rawData.Append($"&redirectUrl={(returnUrl ?? _opts.ReturnUrl)}");
            rawData.Append($"&requestId={requestId}");
            rawData.Append($"&requestType=captureWallet");

            var signature = HmacSha256(_opts.SecretKey, rawData.ToString());

            var postData = new Dictionary<string, object>
            {
                ["partnerCode"] = _opts.PartnerCode,
                ["accessKey"] = _opts.AccessKey,
                ["requestId"] = requestId,
                ["amount"] = amount,
                ["orderId"] = orderId,
                ["orderInfo"] = $"Payment for booking {payment.BookingId}",
                ["redirectUrl"] = (returnUrl ?? _opts.ReturnUrl),
                ["ipnUrl"] = _opts.NotifyUrl,
                ["lang"] = "vi",
                ["extraData"] = "",
                ["requestType"] = "captureWallet",
                ["signature"] = signature
            };

            var content = new StringContent(JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync(_opts.Endpoint, content);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            if (doc.RootElement.TryGetProperty("payUrl", out var payUrlEl))
            {
                return payUrlEl.GetString()!;
            }

            throw new InvalidOperationException("MoMo create order failed");
        }

        public bool ValidateCallback(IDictionary<string, string> payload, out Guid paymentId, out bool success)
        {
            paymentId = Guid.Empty;
            success = false;

            // MoMo callback usually has signature field named "signature"
            if (!payload.TryGetValue("signature", out var received)) return false;

            // Build raw string in the exact order MoMo expects.
            // Example: partnerCode=...&accessKey=...&amount=...&orderId=...&orderInfo=...&orderType=...&transId=...&message=...&localMessage=...&errorCode=0&payType=...&extraData=...
            // You must replicate the exact fields and order from MoMo docs.
            var keysOrder = new[] { "partnerCode", "accessKey", "requestId", "amount", "orderId", "orderInfo", "orderType", "transId", "message", "localMessage", "errorCode", "payType", "extraData" };
            var sb = new StringBuilder();
            foreach (var k in keysOrder)
            {
                if (payload.TryGetValue(k, out var v))
                {
                    if (sb.Length > 0) sb.Append("&");
                    sb.Append($"{k}={v}");
                }
            }

            var calculated = HmacSha256(_opts.SecretKey, sb.ToString());
            if (!string.Equals(calculated, received, StringComparison.OrdinalIgnoreCase)) return false;

            if (payload.TryGetValue("orderId", out var orderId) && Guid.TryParse(orderId, out var pid))
                paymentId = pid;

            if (payload.TryGetValue("errorCode", out var ec) && ec == "0")
                success = true;

            return true;
        }

        private static string HmacSha256(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
