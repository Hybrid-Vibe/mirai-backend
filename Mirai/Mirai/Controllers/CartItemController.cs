using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemsService _cartItemsService;
        private readonly IUserService _userService;
        public CartItemController(ICartItemsService cartItemsService, IUserService userService)
        {
            _cartItemsService = cartItemsService;
            _userService = userService;
        }
        [HttpPost("Create-cart-items")]
        public async Task<IActionResult> CreateCartItem([FromBody] CreateCartDto createCartDto)
        {
            var user = await _userService.GetUserByIdAsync(createCartDto.UserId);
            if (user == null)
            {
                return NotFound($"User not found by ID: {createCartDto.UserId}");
            }
            var result = await _cartItemsService.CreateCartDtoAsync(createCartDto);
            return Ok(result);
        }

        [HttpPost("Checkout-from-cart-{userId}")]
        public async Task<IActionResult> CheckoutFromCart(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User not found by ID: {userId}");
            }
            try
            {
                var result = await _cartItemsService.CheckoutFromCart(userId);
                if (result == null)
                {
                    return NotFound("Cart is empty");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
