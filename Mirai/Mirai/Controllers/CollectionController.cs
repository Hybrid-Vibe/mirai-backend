using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Interfaces;
using System.Threading.Tasks;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionService _collectionService;

        public CollectionController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        [HttpGet("Get-All-Active")]
        public async Task<IActionResult> GetAllActive()
        {
            var collections = await _collectionService.GetAllActiveAsync();
            return Ok(collections);
        }

        [HttpGet("Get-By-Slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var collection = await _collectionService.GetBySlugAsync(slug);
            if (collection == null)
            {
                return NotFound($"Collection with slug '{slug}' not found.");
            }
            return Ok(collection);
        }
    }
}
