using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Extension;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.SearchFilter;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
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

        public async Task<PagedResult<GetAllProductsByFilterDto>> GetProductsByFilterAsync(ProductSearchFilter filter)
        {
            var query = _context.Products.AsQueryable()
                .Where(p => p.IsActive == true)
                .Select(p => new GetAllProductsByFilterDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    BrandId = p.BrandId,
                    BrandName = p.Brand != null ? p.Brand.BrandName : null,
                    RatingAvg = p.RatingAvg,
                    RatingCount = p.RatingCount,
                    Variants = p.ProductVariants
                        .Where(v => v.IsActive == true)
                        .Select(v => new GetAllProductVariantsByFilterDto
                        {
                            VariantId = v.VariantId,
                            Color = v.Color,
                            PhoneModel = v.PhoneModel,
                            Price = v.Price,
                            Stock = v.Stock
                        })
                        .ToList(),
                    ProductImages = p.ProductImages
                        .Select(pi => new ProductImageDto
                        {
                            ImageId = pi.ImageId,
                            ImageUrl = pi.ImageUrl,
                            AltText = pi.AltText,
                        })
                        .ToList()
                });
            if (!string.IsNullOrEmpty(filter.ProductId))
            {
                query = query.Where(u => u.ProductId.Contains(filter.ProductId));
            }
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(u => u.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(u => u.Description.Contains(filter.Description));
            }
            if (!string.IsNullOrEmpty(filter.CategoryId))
            {
                query = query.Where(u => u.CategoryId.Contains(filter.CategoryId));
            }
            if (!string.IsNullOrEmpty(filter.CategoryName))
            {
                query = query.Where(u => u.CategoryName.Contains(filter.CategoryName));
            }
            if (!string.IsNullOrEmpty(filter.BrandId))
            {
                query = query.Where(u => u.BrandId.Contains(filter.BrandId));
            }
            if (!string.IsNullOrEmpty(filter.BrandName))
            {
                query = query.Where(u => u.BrandName.Contains(filter.BrandName));
            }
            if (!string.IsNullOrEmpty(filter.VariantId))
            {
                query = query.Where(u => u.Variants.Any(v => v.VariantId.Contains(filter.VariantId)));
            }
            if (!string.IsNullOrEmpty(filter.Color))
            {
                query = query.Where(u => u.Variants.Any(v => v.Color.Contains(filter.Color)));
            }
            if (!string.IsNullOrEmpty(filter.PhoneModel))
            {
                query = query.Where(u => u.Variants.Any(v => v.PhoneModel.Contains(filter.PhoneModel)));
            }
            if (filter.FromPrice.HasValue)
            {
                query = query.Where(u => u.Variants.Any(v => v.Price >= filter.FromPrice.Value));
            }
            if (filter.ToPrice.HasValue)
            {
                query = query.Where(u => u.Variants.Any(v => v.Price <= filter.ToPrice.Value));
            }

            var totalCount = await query.CountAsync();

            var colors = await query
                .SelectMany(u => u.Variants.Select(v => v.Color))
                .Distinct()
                .ToListAsync();

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)    
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<GetAllProductsByFilterDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,     
                PageSize = filter.PageSize,
                Colors = colors
            };

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
