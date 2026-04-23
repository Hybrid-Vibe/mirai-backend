using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
         private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("Get-All-Categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);

        }
        [HttpGet("Get-All-Categories-Active")]
        public async Task<IActionResult> GetAllCategoriesActive()
        {
            var categories = await _categoryService.GetAllCategoriesActiveAsync();
            return Ok(categories);

        }
        [HttpGet("Get-Category-By-Id/{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound($"Category not found by id: {id}");
            }
            return Ok(category);
        }
        [HttpGet("Get-Category-Active-By-Id/{id}")]
        public async Task<IActionResult> GetCategoryActiveById(string id)
        {
            var category = await _categoryService.GetCategoryActiveByIdAsync(id);
            if (category == null)
            {
                return NotFound($"Category not found by id: {id}");
            }
            return Ok(category);
        }

        [HttpPost("Create-Category")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            var category = await _categoryService.CreateCategoryAsync(categoryDto);
            return Ok(category);
        }

        [HttpPut("Update-Category/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryDto categoryDto)
        {
            var category = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (category == null)
            {
                return NotFound($"Category not found by id: {id}");
            }
            return Ok(category);
        }

        [HttpPut("Delete-Category/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound($"Category not found by id: {id}");
            }
            return Ok("Deleted successfully");
        }
    }
}
