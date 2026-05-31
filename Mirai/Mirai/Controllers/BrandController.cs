using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        public readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [Authorize(Roles = "1, 2, 3")]
        [HttpGet("Get-All-Brands")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllAsync();
            if(brands.Count < 0)
            {
                return NotFound("No brands found");
            }
            return Ok(brands);
        }
        [HttpGet("Get-All-Brands-Active")]
        public async Task<IActionResult> GetAllBrandsActive()
        {
            var brands = await _brandService.GetAllBrandsActiveAsync();
            if (brands.Count == 0)
            {
                return NotFound("No brands found");
            }
            return Ok(brands);
        }
        [HttpGet("Get-Brand-By-Id/{brandId}")]
        public async Task<IActionResult> GetBrandById(string brandId)
        {
            var brand = await _brandService.GetByIdAsync(brandId);
            if (brand == null)
            {
                return NotFound($"No brand found by ID: {brandId}");
            }
            return Ok(brand);
        }
        [HttpGet("Get-Brand-Active-By-Id/{brandId}")]
        public async Task<IActionResult> GetBrandActiveById(string brandId)
        {
            var brand = await _brandService.GetBrandActiveByIdAsync(brandId);
            if (brand == null)
            {
                return NotFound($"No brand found by ID: {brandId}");
            }
            return Ok(brand);
        }

        [HttpPost("Create-Brand")]
        public async Task<IActionResult> CreateBrand([FromBody] BrandDto brandDto)
        {
            var brand = await _brandService.CreateBrandAsync(brandDto);
            return Ok(brand);
        }
        [HttpPut("Update-Brand/{brandId}")]
        public async Task<IActionResult> UpdateBrand(string brandId, [FromBody] BrandDto brandDto)
        {
            var brand = await _brandService.UpdateBrandAsync(brandId, brandDto);
            return Ok(brand);
        }
        [HttpPut("Delete-Brand/{brandId}")]
        public async Task<IActionResult> DeleteBrand(string brandId)
        {
            var result = await _brandService.DeleteBrandAsync(brandId);
            if (!result)
            {
                return NotFound($"No brand found by ID: {brandId}");
            }
            return Ok($"Brand with ID: {brandId} has been deleted");
        }
    }
}
