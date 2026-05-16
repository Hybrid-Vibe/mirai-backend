using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class BrandService : IBrandService
    {
        public readonly IUnitOfWork _unitOfWork;
        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Brand> CreateBrandAsync(BrandDto brandDto)
        {
            return await _unitOfWork.BrandRepository.CreateBrandAsync(brandDto);
        }

        public async Task<bool> DeleteBrandAsync(string id)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            return await _unitOfWork.BrandRepository.DeleteBrandAsync(id);
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _unitOfWork.BrandRepository.GetAllAsync();
        }

        public async Task<List<Brand>> GetAllBrandsActiveAsync()
        {
            return await _unitOfWork.BrandRepository.GetAllBrandsActiveAsync();
        }

        public async Task<Brand?> GetBrandActiveByIdAsync(string id)
        {
            return await _unitOfWork.BrandRepository.GetBrandActiveByIdAsync(id);
        }

        public async Task<Brand> GetByIdAsync(string id)
        {
            return await _unitOfWork.BrandRepository.GetByIdAsync(id);
        }

        public async Task<Brand> UpdateBrandAsync(string id, BrandDto brandDto)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }
            return await _unitOfWork.BrandRepository.UpdateBrandAsync(id, brandDto);
        }
    }
}
