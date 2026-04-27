using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("Create-Payment-Url")]
        public async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = await _paymentService.CreatePaymentUrl(model, HttpContext);
            return Ok(new { PaymentUrl = url });
        }
        [HttpGet("Callback")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = await _paymentService.PaymentExecute(Request.Query);
            return new JsonResult(response);
        }

    }
}
