using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashSalesController : ControllerBase
    {
        private readonly IFlashSaleService _flashSaleService;
        public FlashSalesController(IFlashSaleService flashSaleService)
        {
            _flashSaleService = flashSaleService;
        }

        [HttpPost("Flash-Sales")]
        public async Task<IActionResult> CreateFlashSaleDtosAsync([FromBody] CreateFlashSaleRequestDto createFlashSaleRequestDto)
        {
            var flashSaleDto = await _flashSaleService.CreateFlashSaleDtosAsync(createFlashSaleRequestDto);
            return Ok(flashSaleDto);
        }

        [HttpPut("Flash-Sales/{flashSaleId}")]
        public async Task<IActionResult> UpdateFlashSaleAsync(string flashSaleId, [FromBody] UpdateFlashSaleRequest request)
        {
            await _flashSaleService.UpdateFlashSaleAsync(flashSaleId, request);
            return Ok($"Update {flashSaleId} successful");
        }

        [HttpDelete("Flash-Sales/{flashSaleId}")]
        public async Task<IActionResult> DeleteFlashSaleAsync(string flashSaleId)
        {
            await _flashSaleService.DeleteFlashSaleAsync(flashSaleId);
            return Ok($"Delete {flashSaleId} successful");
        }
    }
}
