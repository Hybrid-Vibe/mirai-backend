using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using Mirai.Application.SearchFilter;

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

        [HttpPost("Checkout-from-cart/{userId}")]
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
        [HttpGet("Get-cart-by-id")]
        public async Task<IActionResult> GetCartById([FromQuery] CartSearchFilter filter)
        {
            var result = await _cartItemsService.GetCartById(filter);
            if (result == null)
            {
                return NotFound($"Cart not found by ID: {filter.CartId}");
            }
            return Ok(result);
        }

        [HttpDelete("Delete-cart-item/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(string cartItemId)
        {
            var wasDeleted = await _cartItemsService.DeleteCartItem(cartItemId);

            if (!wasDeleted)
            {
                return NotFound("Cart item not found");
            }
            
            return Content($"Cart item with ID: {cartItemId} has been deleted");
        }

        [HttpGet("Get-cart-item-by-id/{cartItemId}")]
        public async Task<IActionResult> GetCartItemById(string cartItemId)
        {
            var cartItem = await _cartItemsService.GetByCartItemIdAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound($"Cart item not found by ID: {cartItemId}");
            }
            return Ok(cartItem);
        }
    }
}
