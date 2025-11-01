using Microsoft.Extensions.Options;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PawNest.BLL.Services.Implements
{
    public class VnPayOptions
    {
        public string VnpUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public string TmnCode { get; set; } = null!;
        public string HashSecret { get; set; } = null!; // secret key for HMAC SHA512
    }
    public class VnPayService : IVnPayService
    {

        private readonly VnPayOptions _opts;

        public VnPayService(IOptions<VnPayOptions> opts)
        {
            _opts = opts.Value;
        }

        public string CreatePaymentUrl(Payment payment, string? returnUrl = null)
        {
            var vnpParams = new Dictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", _opts.TmnCode },
            { "vnp_Amount", ((long)(payment.Amount * 100)).ToString() }, // amount in cents
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", payment.PaymentId.ToString() },
            { "vnp_OrderInfo", $"Payment for booking {payment.BookingId}" },
            { "vnp_OrderType", "other" },
            { "vnp_Locale", "vn" },
            { "vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss") },
            { "vnp_ReturnUrl", returnUrl ?? _opts.ReturnUrl }
        };

            // Build data string sorted by key
            var sorted = vnpParams.OrderBy(kv => kv.Key, StringComparer.Ordinal);
            var queryString = string.Join("&", sorted.Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}"));
            var hashData = string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));

            var secureHash = HmacSha512(_opts.HashSecret, hashData);
            var url = $"{_opts.VnpUrl}?{queryString}&vnp_SecureHash={secureHash}";

            return url;
        }

        public bool ValidateCallback(IDictionary<string, string> payload, out Guid paymentId, out bool success)
        {
            paymentId = Guid.Empty;
            success = false;

            if (!payload.TryGetValue("vnp_SecureHash", out var receivedHash)) return false;

            // Remove secure hash from collection to build hashData
            var dict = payload
                .Where(kv => !string.Equals(kv.Key, "vnp_SecureHash", StringComparison.OrdinalIgnoreCase)
                          && !string.Equals(kv.Key, "vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            var sorted = dict.OrderBy(kv => kv.Key, StringComparer.Ordinal);
            var hashData = string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));

            var calculated = HmacSha512(_opts.HashSecret, hashData);
            if (!string.Equals(calculated, receivedHash, StringComparison.OrdinalIgnoreCase)) return false;

            // parse payment id from txn ref
            if (dict.TryGetValue("vnp_TxnRef", out var txnRef) && Guid.TryParse(txnRef, out var pid))
                paymentId = pid;

            // vnp_ResponseCode == "00" means success
            if (dict.TryGetValue("vnp_ResponseCode", out var r) && r == "00")
                success = true;

            return true;
        }

        private static string HmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
