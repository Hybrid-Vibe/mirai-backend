using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IStorageService _storageService;
        public ImagesController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("Upload-Image")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var url = await _storageService.UploadImage(file);
            return Ok(new { imageUrl = url });
        }
    }
}
