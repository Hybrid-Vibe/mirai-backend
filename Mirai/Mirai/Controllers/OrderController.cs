using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
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

        [HttpGet("Orders-History-By-User/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            var orders = await _orderService.GetByUserIdAsync(userId);
            return Ok(orders ?? new List<Order>());
        }

        [Authorize(Roles = "1")]
        [HttpPut("Update-Order-Status/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] OrderStatus newStatus)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found");
                }
                var result = await _orderService.UpdateOrderStatus(id, newStatus);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Status update failed");
            }
            
        }

        [Authorize(Roles = "1")]
        [HttpPut("Update-Payment-Status/{id}")]
        public async Task<IActionResult> UpdatePaymentStatus(string id, [FromBody] PaymentStatus newStatus)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found");
                }
                var result = await _orderService.UpdatePaymentStatus(id, newStatus);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Status update failed");
            }
        }


    }
}
