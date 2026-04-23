using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<Brand>> GetAllAsync()
        {
            return base.GetAllAsync();
        }
        public Task<Brand> GetByIdAsync(string id)
        {
            return base.GetByIdAsync(id);
        }

        public async Task<Brand> CreateBrandAsync(BrandDto brandDto)
        {
            var brand = new Brand
            {
                BrandId = Guid.NewGuid().ToString(),
                BrandName = brandDto.BrandName,
                Description = brandDto.Description,
                //IsActive = true,
                CreatedAt = DateTime.Now
            };
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<bool> DeleteBrandAsync(string id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            brand.IsActive = false;

            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Brand>> GetAllBrandsActiveAsync()
        {
            return await _context.Brands.Where(b => b.IsActive == true).ToListAsync();
        }

        public async Task<Brand> UpdateBrandAsync(string id, BrandDto brandDto)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            brand.BrandName = brandDto.BrandName;
            brand.Description = brandDto.Description;
            brand.UpdatedAt = DateTime.Now;

            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return brand;
        }

        public async Task<Brand?> GetBrandActiveByIdAsync(string id)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id && b.IsActive == true);
        }
    }
}
