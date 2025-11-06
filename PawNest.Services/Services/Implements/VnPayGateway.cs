using Microsoft.Extensions.Configuration;
using PawNest.Services.Services.Interfaces;
using PawNest.Repository.Data.Entities;
using PawNest.Repository.Data.Requests.Payment;
using PawNest.Repository.Data.Responses.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PawNest.Services.Services.Implements
{
    public class VnPayGateway : IPaymentGateway
    {
        private readonly string _vnpUrl;
        private readonly string _vnpTmnCode;
        private readonly string _vnpHashSecret;
        private readonly string _vnpVersion = "2.1.0";

        public VnPayGateway(IConfiguration configuration)
        {
            _vnpUrl = configuration["VNPay:Url"];
            _vnpTmnCode = configuration["VNPay:TmnCode"];
            _vnpHashSecret = configuration["VNPay:HashSecret"];
        }

        public Task<PaymentGatewayResponse> CreatePaymentUrl(PaymentGatewayRequest request)
        {
            var vnpay = new VNPayLibrary();

            vnpay.AddRequestData("vnp_Version", _vnpVersion);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _vnpTmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(request.Amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", request.IpAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", request.OrderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", request.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", request.BookingId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(_vnpUrl, _vnpHashSecret);

            return Task.FromResult(new PaymentGatewayResponse
            {
                Success = true,
                PaymentUrl = paymentUrl
            });
        }

        public Task<PaymentCallbackResponse> ProcessCallback(Dictionary<string, string> queryParams)
        {
            var vnpay = new VNPayLibrary();

            foreach (var (key, value) in queryParams)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }

            string vnp_SecureHash = queryParams["vnp_SecureHash"];
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _vnpHashSecret);

            if (!checkSignature)
            {
                return Task.FromResult(new PaymentCallbackResponse
                {
                    Success = false,
                    Message = "Invalid signature"
                });
            }

            string responseCode = queryParams["vnp_ResponseCode"];
            string transactionId = queryParams["vnp_TransactionNo"];
            decimal amount = decimal.Parse(queryParams["vnp_Amount"]) / 100;

            var status = responseCode == "00" ? PaymentStatus.Success : PaymentStatus.Failed;

            return Task.FromResult(new PaymentCallbackResponse
            {
                Success = responseCode == "00",
                TransactionId = transactionId,
                Amount = amount,
                Status = status,
                Message = GetVNPayResponseMessage(responseCode)
            });
        }

        public async Task<PaymentQueryResponse> QueryPayment(string transactionId)
        {
            // Implement VNPay query transaction API
            var vnpay = new VNPayLibrary();

            vnpay.AddRequestData("vnp_Version", _vnpVersion);
            vnpay.AddRequestData("vnp_Command", "querydr");
            vnpay.AddRequestData("vnp_TmnCode", _vnpTmnCode);
            vnpay.AddRequestData("vnp_TxnRef", orderId);
            vnpay.AddRequestData("vnp_OrderInfo", $"Query transaction {orderId}");
            vnpay.AddRequestData("vnp_TransactionDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            var apiUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";

            string requestUrl = vnpay.CreateRequestUrl(apiUrl, _vnpHashSecret);

            using var client = new HttpClient();
            var response = await client.GetStringAsync(requestUrl);

            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(response);

            string responseCode = data["vnp_ResponseCode"];

            return new PaymentQueryResponse
            {
                Success = responseCode == "00",
                Status = responseCode == "00" ? PaymentStatus.Success : PaymentStatus.Failed,
                Message = GetVNPayResponseMessage(responseCode)
            };
        }

        private string GetVNPayResponseMessage(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
                "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP).",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
                "75" => "Ngân hàng thanh toán đang bảo trì.",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định.",
                _ => "Giao dịch thất bại"
            };
        }
    }
    public class VNPayLibrary
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>();
        private SortedList<string, string> _responseData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (var kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string queryString = data.ToString();
            if (queryString.EndsWith("&"))
            {
                queryString = queryString.Substring(0, queryString.Length - 1);
            }

            string signData = queryString;
            string vnpSecureHash = HmacSHA512(vnpHashSecret, signData);

            return $"{baseUrl}?{queryString}&vnp_SecureHash={vnpSecureHash}";
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            StringBuilder data = new StringBuilder();
            foreach (var kv in _responseData)
            {
                if (!string.IsNullOrEmpty(kv.Value) && kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string signData = data.ToString();
            if (signData.EndsWith("&"))
            {
                signData = signData.Substring(0, signData.Length - 1);
            }

            string myChecksum = HmacSHA512(secretKey, signData);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSHA512(string key, string data)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }
    }
}
