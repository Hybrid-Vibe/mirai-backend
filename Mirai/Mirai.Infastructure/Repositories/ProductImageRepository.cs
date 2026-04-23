using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Mirai.Infastructure.Repositories
{
    public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ProductImage> CreateProductImage(CreateProductImageDto createProductImageDto)
        {
            var productImage = new ProductImage()
            {
                ImageId = Guid.NewGuid().ToString(),
                ProductId = createProductImageDto.ProductId,
                VariantId = createProductImageDto.VariantId,
                ImageUrl = createProductImageDto.ImageUrl,
                AltText = createProductImageDto.AltText,
            };
            await _context.ProductImages.AddAsync(productImage);
            await _context.SaveChangesAsync();
            return productImage;
        }

        public async Task<ProductImage?> GetProductImageById(string imageId)
        {
            return await _context.ProductImages.FirstOrDefaultAsync(x => x.ImageId == imageId);
        }

        public async Task<ProductImage> UpdateProductImage(string productImageId, CreateProductImageDto createProductImageDto)
        {
            var productImage = await _context.ProductImages.FirstOrDefaultAsync(x => x.ImageId == productImageId);
            if (productImage == null)
            {
                return null;
            }
            productImage.ProductId = createProductImageDto.ProductId;
            productImage.VariantId = createProductImageDto.VariantId;
            productImage.ImageUrl = createProductImageDto.ImageUrl;
            productImage.AltText = createProductImageDto.AltText;
            _context.ProductImages.Update(productImage);
            await _context.SaveChangesAsync();
            return productImage;
        }
    }
}
