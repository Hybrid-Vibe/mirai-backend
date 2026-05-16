using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IProductVariantService
    {
        Task<ProductVariant> CreateProductVariant(CreateProductVariantDto createProductVariantDto);
        Task<ProductVariant> UpdateProductVariant(string productVariantId, CreateProductVariantDto createProductVariantDto);
        Task<ProductVariant?> GetProductVariantById(string productVariantId);
    }
}
