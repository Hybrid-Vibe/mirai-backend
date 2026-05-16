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
    public class ProductVariantRepository : GenericRepository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ProductVariant> CreateProductVariant(CreateProductVariantDto createProductVariantDto)
        {
            var productVariant = new ProductVariant()
            {
                VariantId = Guid.NewGuid().ToString(),
                ProductId = createProductVariantDto.ProductId,
                Color = createProductVariantDto.Color,
                PhoneModel = createProductVariantDto.PhoneModel,
                Price = createProductVariantDto.Price,
                CompareAtPrice = createProductVariantDto.CompareAtPrice,
                CostPrice = createProductVariantDto.CostPrice,
                //Barcode = createProductVariantDto.Barcode,
                Stock = createProductVariantDto.Stock,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            await _context.ProductVariants.AddAsync(productVariant);
            await _context.SaveChangesAsync();
            return productVariant;
        }

        public async Task<ProductVariant?> GetProductVariantById(string productVariantId)
        {
            return await _context.ProductVariants.FirstOrDefaultAsync(x => x.VariantId == productVariantId);
        }

        public async Task<ProductVariant> UpdateProductVariant(string productVariantId, CreateProductVariantDto createProductVariantDto)
        {
            var productVariant = await _context.ProductVariants.FirstOrDefaultAsync(x => x.VariantId == productVariantId);
            if (productVariant == null)
            {
                throw new Exception("Product variant not found");
            }

            productVariant.Color = createProductVariantDto.Color;
            productVariant.PhoneModel = createProductVariantDto.PhoneModel;
            productVariant.Price = createProductVariantDto.Price;
            productVariant.CompareAtPrice = createProductVariantDto.CompareAtPrice;
            productVariant.CostPrice = createProductVariantDto.CostPrice;
            //productVariant.Barcode = createProductVariantDto.Barcode;
            productVariant.Stock = createProductVariantDto.Stock;
            productVariant.UpdatedAt = DateTime.Now;

            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();

            return productVariant;
        }
    }
}
