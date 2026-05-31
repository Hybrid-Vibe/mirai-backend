using Mirai.Application.DTO;
using Mirai.Application.Extension;
using Mirai.Application.SearchFilter;
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
        Task<bool> DeleteCartItem(string cartItemId);
        Task<bool> DeleteCart(string cartId);
        Task<CartItem?> GetByCartItemIdAsync(string id);
        Task<PagedResult<CartDto>> GetCartById(CartSearchFilter filter);
    }
}
