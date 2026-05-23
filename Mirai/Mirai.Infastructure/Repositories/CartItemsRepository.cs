using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Extension;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.SearchFilter;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

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

        public async Task<bool> DeleteCart(string cartId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }



        public async Task<bool> DeleteCartItem(string cartItemId)
        {
            var cartItem = await _context.CartItems
        .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
            {
                return false;
            }

            var cartId = cartItem.CartId;

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            var hasItems = await _context.CartItems
                .AnyAsync(ci => ci.CartId == cartId);

            if (!hasItems)
            {
                await DeleteCart(cartId);
            }
            return true;
        }

        public async Task<PagedResult<CartDto>> GetCartById(CartSearchFilter filter)
        {
            var query = _context.Carts.AsQueryable();

            if (!string.IsNullOrEmpty(filter.CartId))
            {
                query = query.Where(c => c.CartId == filter.CartId);
            }
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                query = query.Where(c => c.UserId == filter.UserId);
            }

            var result = await query
                .Select(c => new CartDto
                {
                    CartId = c.CartId,
                    Items = c.CartItems.Where(ci => ci.CartId == c.CartId)
                        .Join(_context.ProductVariants,
                            ci => ci.VariantId,
                            v => v.VariantId,
                            (ci, v) => new { ci, v })
                        .Join(_context.Products,
                            cv => cv.v.ProductId,
                            p => p.ProductId,
                            (cv, p) => new { cv.ci, cv.v, p })
                        .GroupJoin(_context.ProductImages.Where(i => i.IsPrimary == true),
                            cpi => cpi.p.ProductId,
                            img => img.ProductId,
                            (cpi, img) => new { cpi, img })
                        .SelectMany(
                            x => x.img.DefaultIfEmpty(),
                            (x, img) => new CartItemDto
                            {
                                CartItemId = x.cpi.ci.CartItemId,
                                VariantId = x.cpi.ci.VariantId,
                                ProductName = x.cpi.p.Name,
                                Image = img != null ? img.ImageUrl : null,
                                Price = x.cpi.ci.UnitPrice ?? x.cpi.v.Price ?? 0,
                                Quantity = x.cpi.ci.Quantity,
                                Total = (x.cpi.ci.UnitPrice ?? x.cpi.v.Price ?? 0) * x.cpi.ci.Quantity
                            }).ToList(),
                    TotalPrice = _context.CartItems
                        .Where(ci => ci.CartId == c.CartId)
                        .Join(_context.ProductVariants,
                            ci => ci.VariantId,
                            v => v.VariantId,
                            (ci, v) => (ci.UnitPrice ?? v.Price ?? 0) * ci.Quantity
                        )
                        .Sum()
                })
                .FirstOrDefaultAsync();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(c => new CartDto
                {
                    CartId = c.CartId,
                    Items = c.CartItems.Where(ci => ci.CartId == c.CartId)
                        .Join(_context.ProductVariants,
                            ci => ci.VariantId,
                            v => v.VariantId,
                            (ci, v) => new { ci, v })
                        .Join(_context.Products,
                            cv => cv.v.ProductId,
                            p => p.ProductId,
                            (cv, p) => new { cv.ci, cv.v, p })
                        .GroupJoin(_context.ProductImages.Where(i => i.IsPrimary == true),
                            cpi => cpi.p.ProductId,
                            img => img.ProductId,
                            (cpi, img) => new { cpi, img })
                        .SelectMany(
                            x => x.img.DefaultIfEmpty(),
                            (x, img) => new CartItemDto
                            {
                                CartItemId = x.cpi.ci.CartItemId,
                                VariantId = x.cpi.ci.VariantId,
                                ProductName = x.cpi.p.Name,
                                Image = img != null ? img.ImageUrl : null,
                                Price = x.cpi.ci.UnitPrice ?? x.cpi.v.Price ?? 0,
                                Quantity = x.cpi.ci.Quantity,
                                Total = (x.cpi.ci.UnitPrice ?? x.cpi.v.Price ?? 0) * x.cpi.ci.Quantity
                            }).ToList(),
                    TotalPrice = _context.CartItems
                        .Where(ci => ci.CartId == c.CartId)
                        .Join(_context.ProductVariants,
                            ci => ci.VariantId,
                            v => v.VariantId,
                            (ci, v) => (ci.UnitPrice ?? v.Price ?? 0) * ci.Quantity
                        )
                        .Sum()
                })
                .ToListAsync();

            return new PagedResult<CartDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
            };
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

        public async Task<CartItem?> GetByCartItemIdAsync(string id)
        {
            return await _context.CartItems.FirstOrDefaultAsync(item => item.CartItemId == id);
        }
    }
}
