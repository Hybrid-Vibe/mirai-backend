using Mirai.Application.DTO;
using Mirai.Application.Extension;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Application.SearchFilter;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> CreateProduct(CreateProductDto createProductDto)
        {
            return await _unitOfWork.ProductRepository.CreateProduct(createProductDto);
        }

        public async Task CreateAllProducts(CreateProductRequestDto request)
        {
             await _unitOfWork.ProductRepository.CreateProduct(request);
        }

        public async Task<List<ProductDto>> GetAllProducts()
        {
            return await _unitOfWork.ProductRepository.GetAllProducts();
        }

        public async Task<Product?> GetProductById(string productId)
        {
            var product = await _unitOfWork.ProductRepository.GetProductById(productId);
            if (product == null)
            {
                return null;
            }
            return product;
        }

        public async Task<PagedResult<GetAllProductsByFilterDto>> GetProductsByFilterAsync(ProductSearchFilter filter)
        {
            return await _unitOfWork.ProductRepository.GetProductsByFilterAsync(filter);
        }

        public async Task<Product> UpdateProduct(string productId, CreateProductDto createProductDto)
        {
            var product = await _unitOfWork.ProductRepository.GetProductById(productId);
            if (product == null)
            {
                return null;
            }
            return await _unitOfWork.ProductRepository.UpdateProduct(productId, createProductDto);
        }
    }
}
