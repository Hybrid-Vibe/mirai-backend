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
                CategoryId = createProductDto.CategoryId,
                BrandId = createProductDto.BrandId,
                IsActive = true,
                CreatedAt = DateTime.Now,
            };
            await _context.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task CreateProduct(CreateProductRequestDto request)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    var productId = Guid.NewGuid().ToString();
                    var product = new Product()
                    {
                        ProductId = productId,
                        Name = request.Name,
                        Description = request.Description,
                        CategoryId = request.CategoryId,
                        BrandId = request.BrandId,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                    };

                    await _context.AddAsync(product);

                    var productImages = request.Images.Select(x => new ProductImage
                    {
                        ImageId = Guid.NewGuid().ToString(),
                        ProductId = productId,
                        VariantId = null,
                        ImageUrl = x.ImageUrl,
                        CreatedAt = DateTime.Now,
                    }).ToList();

                    await _context.AddRangeAsync(productImages);

                    //variant
                    foreach (var variantDto in request.Variants)
                    {
                        var variantId = Guid.NewGuid().ToString();
                        var variant = new ProductVariant()
                        {
                            VariantId = variantId,
                            ProductId = productId,
                            Color = variantDto.Color,
                            PhoneModel = variantDto.PhoneModel,
                            Price = variantDto.Price,
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                        };

                        await _context.AddRangeAsync(variant);

                        var variantImage = new ProductImage()
                        {
                            ImageId = Guid.NewGuid().ToString(),
                            ProductId = productId,
                            VariantId = variantId,
                            ImageUrl = variantDto.ImageUrl,
                            IsPrimary = false,
                            CreatedAt = DateTime.Now,
                        };
                        await _context.AddRangeAsync(variantImage);

                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            var products = await _context.Products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
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

        public async Task<List<GetFlashSaleProductsDto>> GetFlashSaleProductsAsync()
        {
            var now = DateTime.Now;
            return await _context.FlashSaleItems
                .Where(fsi =>
                    fsi.IsActive == true &&
                    fsi.FlashSale!.IsActive == true &&
                    now >= fsi.FlashSale.StartTime &&
                    now <= fsi.FlashSale.EndTime
                )
                .Select(fsi => new GetFlashSaleProductsDto
                {
                    ProductId = fsi.Variant!.Product!.ProductId,
                    ProductName = fsi.Variant.Product.Name,

                    VariantId = fsi.VariantId,
                    Color = fsi.Variant.Color,
                    PhoneModel = fsi.Variant.PhoneModel,

                    OriginalPrice = (decimal)fsi.Variant.Price,
                    FlashSalePrice = fsi.SalePrice,

                    Stock = (int)fsi.Variant.Stock,

                    ImageUrl = fsi.Variant.Product.ProductImages
                        .Select(x => x.ImageUrl)
                        .FirstOrDefault(),

                    StartTime = fsi.FlashSale.StartTime,
                    EndTime = fsi.FlashSale.EndTime
                })
                .ToListAsync();
        }


        public async Task<Product?> GetProductById(string productId)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<PagedResult<GetAllProductsByFilterDto>> GetProductsByFilterAsync(ProductSearchFilter filter)
        {
            var now = DateTime.Now;
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
                            Stock = v.Stock,
                            FlashSalePrice = _context.FlashSaleItems
                            .Where(fsi => fsi.VariantId == v.VariantId && 
                            fsi.IsActive == true && 
                            fsi.FlashSale!.IsActive == true 
                            && now >= fsi.FlashSale.StartTime && now <= fsi.FlashSale.EndTime).Select(fsi => (decimal?)fsi.SalePrice).FirstOrDefault(),
                            IsFlashSale = _context.FlashSaleItems.Any(fsi => fsi.VariantId == v.VariantId && fsi.IsActive == true && fsi.FlashSale!.IsActive == true && now >= fsi.FlashSale.StartTime && now <= fsi.FlashSale.EndTime),
                            FlashSaleStartTime = _context.FlashSaleItems.Where(fsi => fsi.VariantId == v.VariantId && fsi.IsActive == true && fsi.FlashSale!.IsActive == true).Select(fsi => (DateTime?)fsi.FlashSale.StartTime).FirstOrDefault(),
                            FlashSaleEndTime = _context.FlashSaleItems.Where(fsi => fsi.VariantId == v.VariantId && fsi.IsActive == true && fsi.FlashSale!.IsActive == true).Select(fsi => (DateTime?)fsi.FlashSale.EndTime).FirstOrDefault()
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
            /*if (!string.IsNullOrEmpty(filter.VariantId))
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
*/
            var totalCount = await query.CountAsync();


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
            product.BrandId = createProductDto.BrandId;
            product.CategoryId = createProductDto.CategoryId;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateProductStar(string productId, decimal RatingAvg, int RatingCount)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return false;
            }
            product.RatingAvg = RatingAvg;
            product.RatingCount = RatingCount;
            product.UpdatedAt = DateTime.Now;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
