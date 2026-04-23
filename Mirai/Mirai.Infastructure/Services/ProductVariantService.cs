using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductVariantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductVariant> CreateProductVariant(CreateProductVariantDto createProductVariantDto)
        {
            return await _unitOfWork.ProductVariantRepository.CreateProductVariant(createProductVariantDto);
        }

        public async Task<ProductVariant?> GetProductVariantById(string productVariantId)
        {
            return await _unitOfWork.ProductVariantRepository.GetProductVariantById(productVariantId);
        }

        public async Task<ProductVariant> UpdateProductVariant(string productVariantId, CreateProductVariantDto createProductVariantDto)
        {
            var productVariant = await _unitOfWork.ProductVariantRepository.GetProductVariantById(productVariantId);
            if (productVariant == null)
            {
                return null;
            }

               return await _unitOfWork.ProductVariantRepository.UpdateProductVariant(productVariantId, createProductVariantDto);
        }
    }
}
