using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Enum;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("Create-Order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto orderRequestDto)
        {
            var order = await _orderService.CreateOrder(orderRequestDto);
            return Ok(order);
        }

        [HttpGet("Get-Order-By/{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found");
            return Ok(order);
        }

        [HttpPut("Update-Order-Status/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] OrderStatus newStatus)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found");
            }
            await _orderService.UpdateOrderStatus(id, newStatus);
            return Ok("Update order status successfully");
        }

        [HttpPut("Update-Payment-Status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(string id, [FromBody] PaymentStatus newStatus)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found");
            }
            await _orderService.UpdatePaymentStatus(id, newStatus);
            return Ok("Update payment status successfully");
        }
    }
}
