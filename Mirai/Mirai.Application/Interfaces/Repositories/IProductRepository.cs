using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateProduct(CreateProductDto createProductDto);
        Task<Product> UpdateProduct(string productId, CreateProductDto createProductDto);
        Task<Product?> GetProductById(string productId);
        Task<List<ProductDto>> GetAllProducts();
    }
}
