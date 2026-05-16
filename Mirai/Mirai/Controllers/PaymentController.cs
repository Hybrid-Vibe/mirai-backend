using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Enum;
using Mirai.Infastructure.Services;

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

        [HttpGet("Get-Payment-By-Id/{id}")]
        public async Task<IActionResult> GetPaymentById(string id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
                return NotFound($"Payment with ID {id} not found");
            return Ok(payment);
        }

        [HttpPut("Update-Payment-Status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(string id, [FromBody] PaymentStatusInPayment newStatus)
        {
            var order = await _paymentService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found");
            }
            await _paymentService.UpdatePaymentStatus(id, newStatus);
            return Ok("Update payment status successfully");
        }

        [HttpPost("Create-Payment-By-COD")]
        public async Task<IActionResult> CreatePaymentByCOD([FromBody] PaymentByCODDto paymentByCODDto)
        {
            var payment = await _paymentService.CreatePaymentByCOD(paymentByCODDto);
            if (payment == null)
            {
                return BadRequest("Failed to create payment");
            }
            return Ok(payment);
        }
    }
}
