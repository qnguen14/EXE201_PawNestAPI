using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PawNest.BLL.Services.Interfaces;
using PawNest.DAL.Data.Requests.Payment;

namespace PawNest.API.Controllers
{
    
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
            => Ok(await _paymentService.CreatePaymentAsync(request));

        [HttpGet("vnpay/return")]
        public async Task<IActionResult> VNPayReturn()
        {
            var result = await _paymentService.HandleProviderCallbackAsync(
                "vnpay",
                Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString())
            );
            return result ? Ok("Payment successful") : BadRequest("Payment failed");
        }

        [HttpPost("momo/ipn")]
        public async Task<IActionResult> MoMoIpn([FromBody] Dictionary<string, string> payload)
        {
            var result = await _paymentService.HandleProviderCallbackAsync("momo", payload);
            return result ? Ok("IPN received") : BadRequest("Invalid IPN");
        }
    }
}
