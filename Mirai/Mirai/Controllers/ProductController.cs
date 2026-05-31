using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using Mirai.Application.SearchFilter;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("Get-All-Products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(products);
        }
        [HttpGet("Get-Product-By-Id/{productId}")]
        public async Task<IActionResult> GetProductById(string productId)
        {
            var product = await _productService.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("Create-Product")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            var product = await _productService.CreateProduct(createProductDto);
            return Ok(product);
        }

        [HttpPost("Create-Product-ProductImages-ProductVariants")]
        public async Task<IActionResult> CreateAllProducts([FromBody] CreateProductRequestDto request)
        {
            await _productService.CreateAllProducts(request);
            return Ok("product");
        }

        [HttpPut("Update-Product/{productId}")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] CreateProductDto createProductDto)
        {
            var product = await _productService.UpdateProduct(productId, createProductDto);
            if (product == null)
            {
                return NotFound($"Product not found by ID: {productId}");
            }
            return Ok(product);
        }

        [HttpGet("Get-Products-By-Filter")]
        public async Task<IActionResult> GetProductsByFilter([FromQuery] ProductSearchFilter filter)
        {
            var products = await _productService.GetProductsByFilterAsync(filter);
            return Ok(products);
        }

        [HttpGet("Flash-Sale-Products")]
        public async Task<IActionResult> GetFlashSaleProductsAsync()
        {
            var flashSaleProducts = await _productService.GetFlashSaleProductsAsync();
            return Ok(flashSaleProducts);
        }
    }
}
