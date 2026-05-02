using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface ICartItemsRepository
    {
        Task<CreateCartDto> CreateCartDtoAsync(CreateCartDto createCartDto);
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<List<CartItem>> GetCartItemByCartIdAsync(string cartId);
        Task RemoveCartItems(List<CartItem> cartItems);
    }
}
