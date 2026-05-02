using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Repositories
{
    public class CartItemsRepository : GenericRepository<Cart>, ICartItemsRepository
    {
        public CartItemsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<CreateCartDto> CreateCartDtoAsync(CreateCartDto createCartDto)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == createCartDto.UserId);
            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = Guid.NewGuid().ToString(),
                    UserId = createCartDto.UserId,
                    CreatedAt = DateTime.Now
                };
                _context.Carts.Add(cart);
            }

            var existItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.VariantId == createCartDto.VariantId);
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(pv => pv.VariantId == createCartDto.VariantId);
            if (existItem != null) {
                existItem.Quantity += createCartDto.Quantity;
                existItem.UpdatedAt = DateTime.Now;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid().ToString(),
                    CartId = cart.CartId,
                    VariantId = createCartDto.VariantId,
                    Quantity = createCartDto.Quantity,
                    UnitPrice = variant.Price,
                    CreatedAt = DateTime.Now
                };
                await _context.CartItems.AddAsync(cartItem);
            }
            await _context.SaveChangesAsync();
            return createCartDto;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<List<CartItem>> GetCartItemByCartIdAsync(string cartId)
        {
            return await _context.CartItems.Where(ci => ci.CartId == cartId).ToListAsync();
        }

        public async Task RemoveCartItems(List<CartItem> cartItem)
        {
            _context.CartItems.RemoveRange(cartItem);
            await _context.SaveChangesAsync();
        }
    }
}
