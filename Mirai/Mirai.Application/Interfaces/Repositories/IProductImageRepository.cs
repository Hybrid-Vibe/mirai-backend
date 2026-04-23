using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IProductImageRepository
    {
        Task<ProductImage> CreateProductImage(CreateProductImageDto createProductImageDto);
        Task<ProductImage> UpdateProductImage(string productImageId, CreateProductImageDto createProductImageDto);
        Task<ProductImage?> GetProductImageById(string imageId);    

    }
}
