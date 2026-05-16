using Mirai.Application.DTO;
using Mirai.Application.Extension;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Application.SearchFilter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class CartItemsService : ICartItemsService
    {
        public readonly IUnitOfWork _unitOfWork;
        public CartItemsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponseDto> CheckoutFromCart(string userId)
        {
            var cart = await _unitOfWork.CartItemsRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return null;
            }
            var cartItems = await _unitOfWork.CartItemsRepository.GetCartItemByCartIdAsync(cart.CartId);
            if (!cartItems.Any())
            {
                throw new Exception("Cart is empty");
            }

            var orderRequest = new OrderRequestDto
            {
                UserId = userId,
                Note = "Checkout from cart",
                Products = cartItems.Select(ci => new OrderItemRequestDto
                {
                    VariantId = ci.VariantId,
                    Quantity = ci.Quantity
                }).ToList()
            };
            var result = await _unitOfWork.OrderRepository.CreateOrder(orderRequest);
            await _unitOfWork.CartItemsRepository.RemoveCartItems(cartItems);
            return result;
        }

        public async Task<CreateCartDto> CreateCartDtoAsync(CreateCartDto createCartDto)
        {
            var userId = await _unitOfWork.UserRepository.GetUserByIdAsync(createCartDto.UserId);
            if (userId == null)
            {
                return null;
            }
                return await _unitOfWork.CartItemsRepository.CreateCartDtoAsync(createCartDto);
        }

        public async Task<PagedResult<CartDto>> GetCartById(CartSearchFilter filter)
        {
            return await _unitOfWork.CartItemsRepository.GetCartById(filter);
        }
    }
}
