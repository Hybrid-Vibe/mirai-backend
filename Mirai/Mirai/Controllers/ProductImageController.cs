using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _productImageService;
        public ProductImageController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        [HttpGet("Get-ProductImageById/{id}")]
        public async Task<IActionResult> GetProductImageById(string id)
        {
            var productImage = await _productImageService.GetProductImageById(id);
            if (productImage == null)
            {
                return NotFound($"Product image not found by id: {id}");
            }
            return Ok(productImage);
        }

        [HttpPost("Create-ProductImage")]
        public async Task<IActionResult> CreateProductImage([FromBody] CreateProductImageDto createProductImageDto)
        {
            var productImage = await _productImageService.CreateProductImage(createProductImageDto);
            return Ok(productImage);
        }

        [HttpPut("Update-ProductImage/{id}")]
        public async Task<IActionResult> UpdateProductImage(string id, [FromBody] CreateProductImageDto createProductImageDto)
        {
            var productImage = await _productImageService.UpdateProductImage(id, createProductImageDto);
            if (productImage == null)
            {
                return NotFound($"Product image not found by id: {id}");
            }
            return Ok(productImage);
        }
    }
}
