using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetAllCategoriesActiveAsync();
        Task<Category> GetByIdAsync(string id);
        Task<Category?> GetCategoryActiveByIdAsync(string id);
        Task<Category> CreateCategoryAsync(CategoryDto categoryDto);
        Task<Category> UpdateCategoryAsync(string id, CategoryDto categoryDto);
        Task<bool> DeleteCategoryAsync(string id);
    }
}
