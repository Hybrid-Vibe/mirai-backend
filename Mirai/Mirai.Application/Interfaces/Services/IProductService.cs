using Mirai.Application.DTO;
using Mirai.Application.Extension;
using Mirai.Application.SearchFilter;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Product> CreateProduct(CreateProductDto createProductDto);
        Task<Product> UpdateProduct(string productId, CreateProductDto createProductDto);
        Task<Product?> GetProductById(string productId);
        Task<List<ProductDto>> GetAllProducts();
        Task<List<GetFlashSaleProductsDto>> GetFlashSaleProductsAsync();
        Task<PagedResult<GetAllProductsByFilterDto>> GetProductsByFilterAsync(ProductSearchFilter filter);
        Task CreateAllProducts(CreateProductRequestDto request);
    }
}
