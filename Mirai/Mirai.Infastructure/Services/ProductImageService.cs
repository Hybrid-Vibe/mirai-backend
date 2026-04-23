using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Mirai.Infastructure.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductImage> CreateProductImage(CreateProductImageDto createProductImageDto)
        {
            return await _unitOfWork.ProductImageRepository.CreateProductImage(createProductImageDto);
        }

        public async Task<ProductImage?> GetProductImageById(string imageId)
        {
            return await _unitOfWork.ProductImageRepository.GetProductImageById(imageId);
        }

        public async Task<ProductImage> UpdateProductImage(string productImageId, CreateProductImageDto createProductImageDto)
        {
            var productImgId = await _unitOfWork.ProductImageRepository.GetProductImageById(productImageId);
            if (productImgId == null)
            {
                return null;
            }
            var productImage = await _unitOfWork.ProductImageRepository.UpdateProductImage(productImageId, createProductImageDto);
            return productImage;
        }
    }
}
