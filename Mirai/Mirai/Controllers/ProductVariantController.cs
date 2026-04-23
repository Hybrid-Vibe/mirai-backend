using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;
        public ProductVariantController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet("Get-ProductVariant/{productVariantId}")]
        public async Task<IActionResult> GetProductVariantById(string productVariantId)
        {
            var productVariant = await _productVariantService.GetProductVariantById(productVariantId);
            if (productVariant == null)
            {
                return NotFound($"Product variant not found by ID: {productVariantId}.");
            }
            return Ok(productVariant);
        }

        [HttpPost("Create-ProductVariant")]
        public async Task<IActionResult> CreateProductVariant([FromBody] CreateProductVariantDto createProductVariantDto)
        {
            var createdProductVariant = await _productVariantService.CreateProductVariant(createProductVariantDto);
            return Ok(createdProductVariant);
        }
        [HttpPut("Update-ProductVariant/{productVariantId}")]
        public async Task<IActionResult> UpdateProductVariant(string productVariantId, [FromBody] CreateProductVariantDto createProductVariantDto)
        {
            var productVariant = await _productVariantService.GetProductVariantById(productVariantId);
            if (productVariant == null)
            {
                return NotFound($"Product variant not found by ID: {productVariantId}.");
            }
            var updatedProductVariant = await _productVariantService.UpdateProductVariant(productVariantId, createProductVariantDto);
            return Ok(updatedProductVariant);
        }

    }
}