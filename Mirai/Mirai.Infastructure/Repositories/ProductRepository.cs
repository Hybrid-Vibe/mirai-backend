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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Product> CreateProduct(CreateProductDto createProductDto)
        {
            var product = new Product()
            {
                ProductId = Guid.NewGuid().ToString(),
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                CategoryId = createProductDto.CategoryId,
                BrandId = createProductDto.BrandId,
                IsActive = true,
                CreatedAt = DateTime.Now,
            };
            await _context.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var products = await _context.Products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name,
                BrandId = p.BrandId,
                BrandName = p.Brand!.BrandName,
                IsActive = p.IsActive,
                RatingAvg = p.RatingAvg,
                RatingCount = p.RatingCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToListAsync();
            return products;
        }

        public async Task<Product?> GetProductById(string productId)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<Product> UpdateProduct(string productId, CreateProductDto createProductDto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return null;
            }
            product.Name = createProductDto.Name;
            product.Description = createProductDto.Description;
            product.Price = createProductDto.Price;
            product.BrandId = createProductDto.BrandId;
            product.CategoryId = createProductDto.CategoryId;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
