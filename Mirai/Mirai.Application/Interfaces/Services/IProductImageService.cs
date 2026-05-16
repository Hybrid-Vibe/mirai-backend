using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IProductImageService
    {
        Task<ProductImage> CreateProductImage(CreateProductImageDto createProductImageDto);
        Task<ProductImage> UpdateProductImage(string productImageId, CreateProductImageDto createProductImageDto);
        Task<ProductImage?> GetProductImageById(string imageId);
    }
}
