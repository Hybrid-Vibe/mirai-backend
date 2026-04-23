using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllAsync();
        Task<List<Brand>> GetAllBrandsActiveAsync();
        Task<Brand> GetByIdAsync(string id);
        Task<Brand?> GetBrandActiveByIdAsync(string id);
        Task<Brand> CreateBrandAsync(BrandDto brandDto);
        Task<Brand> UpdateBrandAsync(string id, BrandDto brandDto);
        Task<bool> DeleteBrandAsync(string id);
    }
}
