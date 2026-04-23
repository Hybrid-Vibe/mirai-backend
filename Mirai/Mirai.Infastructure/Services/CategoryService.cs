using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class CategoryService : ICategoryService
    {
        public readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category> CreateCategoryAsync(CategoryDto categoryDto)
        {
            return await _unitOfWork.CategoryRepository.CreateCategoryAsync(categoryDto);
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return false;
            }
            return await _unitOfWork.CategoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _unitOfWork.CategoryRepository.GetAllAsync();
        }

        public async Task<List<Category>> GetAllCategoriesActiveAsync()
        {
            return await _unitOfWork.CategoryRepository.GetAllCategoriesActiveAsync();
        }

        public async Task<Category> GetByIdAsync(string id)
        {
            return await _unitOfWork.CategoryRepository.GetByIdAsync(id);
        }

        public async Task<Category?> GetCategoryActiveByIdAsync(string id)
        {
            return await _unitOfWork.CategoryRepository.GetCategoryActiveByIdAsync(id);
        }

        public async Task<Category> UpdateCategoryAsync(string id, CategoryDto categoryDto)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return null;
            }
            return await _unitOfWork.CategoryRepository.UpdateCategoryAsync(id, categoryDto);
        }
    }
}
