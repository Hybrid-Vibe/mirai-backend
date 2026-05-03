using Mirai.Application.DTO;
using Mirai.Application.Extension;
using Mirai.Application.SearchFilter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface ICartItemsService
    {
        Task<CreateCartDto> CreateCartDtoAsync(CreateCartDto createCartDto);
        Task<OrderResponseDto> CheckoutFromCart(string userId);
        Task<PagedResult<CartDto>> GetCartById(CartSearchFilter filter);
    }
}
