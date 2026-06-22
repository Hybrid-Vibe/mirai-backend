using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Enum;
using Mirai.Infastructure.Services;
using System.Text.Json;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost("Create-Payment-Url")]
        public async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = await _paymentService.CreatePaymentUrl(model, HttpContext);
            return Ok(new { PaymentUrl = url });
        }

        [Authorize]
        [HttpPost("PayOS-Url")]
        public async Task<IActionResult> CreatePayOSUrl(string orderId)
        {
            var url = await _paymentService.CreatePayOSUrl(orderId);
            return Ok(new { PaymentUrl = url });
        }
        [HttpGet("Callback")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = await _paymentService.PaymentExecute(Request.Query);
            return new JsonResult(response);
        }

        [HttpPost("payment-webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PayOSWebhookRootDto dto)
        {
            try
            {
                Console.WriteLine("WEBHOOK HIT");
                Console.WriteLine(JsonSerializer.Serialize(dto));
                Console.WriteLine(
                    JsonSerializer.Serialize(dto,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }));
                await _paymentService.HandlePayOSWebhook(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("Get-Payment-By-Id/{id}")]
        public async Task<IActionResult> GetPaymentById(string id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
                return NotFound($"Payment with ID {id} not found");
            return Ok(payment);
        }

        /*[HttpPut("Update-Payment-Status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(string id, [FromBody] PaymentStatusInPayment newStatus)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found");
                }
                var result = await _paymentService.UpdatePaymentStatus(id, newStatus);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Status update failed");
            }
        }*/

        [Authorize]
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
