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
    public class CategoryRepository: GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
        
        public async Task<Category> GetByIdAsync(string id)
        {
            return await base.GetByIdAsync(id);
        }
        public async Task<Category> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                CategoryId = Guid.NewGuid().ToString(),
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (category == null)
            {
                return false;
            }
            category.IsActive = false;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetAllCategoriesActiveAsync()
        {
            return await _context.Categories.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<Category?> GetCategoryActiveByIdAsync(string id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id && x.IsActive);
        }

        public async Task<Category> UpdateCategoryAsync(string id, CategoryDto categoryDto)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (category == null) {
                return null;
            }
            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            category.UpdatedAt = DateTime.Now;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;

        }
    }
}
