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
    public class FlashSaleRepository : GenericRepository<FlashSale>, IFlashSaleRepository
    {
        public FlashSaleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<CreateFlashSaleRequestDto> CreateFlashSaleDtosAsync(CreateFlashSaleRequestDto createFlashSaleRequestDto)
        {
            try
            {
                var flashSaleId = Guid.NewGuid().ToString();
                var flashSale = new FlashSale()
                {
                    FlashSaleId = flashSaleId,
                    Title = createFlashSaleRequestDto.Title,
                    Description = createFlashSaleRequestDto.Description,
                    StartTime = createFlashSaleRequestDto.StartTime.ToLocalTime(),
                    EndTime = createFlashSaleRequestDto.EndTime.ToLocalTime(),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                foreach (var item in createFlashSaleRequestDto.Items)
                {
                    flashSale.FlashSaleItems.Add(new FlashSaleItem()
                    {
                        FlashSaleItemId = Guid.NewGuid().ToString(),
                        FlashSaleId = flashSaleId,
                        VariantId = item.VariantId,
                        SalePrice = item.SalePrice,
                        QuantityLimit = item.QuantityLimit,
                        QuantitySold = 0,
                        PerUserLimit = item.PerUserLimit,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }
                await _context.FlashSales.AddAsync(flashSale);
                await _context.SaveChangesAsync();
                return createFlashSaleRequestDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteFlashSaleAsync(string flashSaleId)
        {
            var flashSale = await _context.FlashSales.FirstOrDefaultAsync(x =>
                    x.FlashSaleId == flashSaleId);

            if (flashSale == null)
                throw new Exception("Flash sale not found");

            flashSale.IsActive = false;

            var items = await _context.FlashSaleItems
                .Where(x => x.FlashSaleId == flashSaleId)
                .ToListAsync();

            foreach (var item in items)
            {
                item.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateFlashSaleAsync(string flashSaleId, UpdateFlashSaleRequest request)
        {
            try
            {
                var flashSale = await _context.FlashSales
                    .Include(x => x.FlashSaleItems)
                    .FirstOrDefaultAsync(x =>
                        x.FlashSaleId == flashSaleId);

                if (flashSale == null)
                {
                    throw new Exception("Flash sale not found");
                }

                // UPDATE FLASH SALE
                flashSale.Title = request.Title;
                flashSale.Description = request.Description;
                flashSale.StartTime = request.StartTime.ToLocalTime();

                flashSale.EndTime = request.EndTime.ToLocalTime();
                flashSale.UpdatedAt = DateTime.Now;

                // EXISTING ITEMS
                var existingItems =
                    flashSale.FlashSaleItems.ToList();

                // UPDATE + ADD
                foreach (var item in request.Items)
                {
                    // UPDATE
                    if (!string.IsNullOrEmpty(item.FlashSaleItemId))
                    {
                        var existingItem = existingItems
                            .FirstOrDefault(x =>
                                x.FlashSaleItemId ==
                                item.FlashSaleItemId);

                        if (existingItem != null)
                        {
                            existingItem.SalePrice =
                                item.SalePrice;

                            existingItem.QuantityLimit =
                                item.QuantityLimit;

                            existingItem.PerUserLimit =
                                item.PerUserLimit;
                            existingItem.IsActive = true;
                        }
                    }
                    // ADD NEW
                    else
                    {
                        flashSale.FlashSaleItems.Add(
                            new FlashSaleItem
                            {
                                FlashSaleItemId =
                                    Guid.NewGuid().ToString(),

                                VariantId = item.VariantId,

                                SalePrice = item.SalePrice,

                                QuantityLimit = item.QuantityLimit,

                                QuantitySold = 0,

                                PerUserLimit = item.PerUserLimit,
                                IsActive = true,
                                CreatedAt = DateTime.Now
                            });
                    }
                }

                // DELETE REMOVED ITEMS
                var requestItemIds = request.Items
                    .Where(x => !string.IsNullOrEmpty(
                        x.FlashSaleItemId))
                    .Select(x => x.FlashSaleItemId)
                    .ToList();

                var deletedItems = existingItems
                    .Where(x =>
                        !requestItemIds.Contains(
                            x.FlashSaleItemId))
                    .ToList();

                _context.FlashSaleItems.RemoveRange(
                    deletedItems);

                await _context.SaveChangesAsync();

            }
            catch
            {

                throw;
            }
        }
    }
}
