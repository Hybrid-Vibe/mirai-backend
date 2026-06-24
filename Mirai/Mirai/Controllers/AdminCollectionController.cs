using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces;
using Mirai.Domain.Enum;
using System;
using System.Threading.Tasks;

namespace Mirai.Controllers
{
    [Route("api/Admin/Collection")]
    [ApiController]
    public class AdminCollectionController : ControllerBase
    {
        private readonly ICollectionService _collectionService;

        public AdminCollectionController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        [HttpPost("Create-Collection")]
        public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionDto createDto)
        {
            var collection = await _collectionService.CreateCollectionAsync(createDto);
            return Ok(collection);
        }

        [HttpPut("Update-Collection/{id}")]
        public async Task<IActionResult> UpdateCollection(Guid id, [FromBody] UpdateCollectionDto updateDto)
        {
            var collection = await _collectionService.UpdateCollectionAsync(id, updateDto);
            if (collection == null)
            {
                return NotFound($"Collection with ID {id} not found.");
            }
            return Ok(collection);
        }

        [HttpDelete("Delete-Collection/{id}")]
        public async Task<IActionResult> DeleteCollection(Guid id)
        {
            var result = await _collectionService.DeleteCollectionAsync(id);
            if (!result)
            {
                return NotFound($"Collection with ID {id} not found.");
            }
            return Ok("Collection deleted successfully.");
        }

        [HttpPost("Add-Products-To-Collection")]
        public async Task<IActionResult> AddProductsToCollection([FromBody] AddProductsToCollectionRequestDto request)
        {
            var result = await _collectionService.AddProductsToCollectionAsync(request);
            if (!result)
            {
                return NotFound($"Collection with ID {request.CollectionId} not found.");
            }
            return Ok("Products added to collection successfully.");
        }

        [HttpPost("Remove-Products-From-Collection")]
        public async Task<IActionResult> RemoveProductsFromCollection([FromBody] RemoveProductsFromCollectionRequestDto request)
        {
            var result = await _collectionService.RemoveProductsFromCollectionAsync(request);
            if (!result)
            {
                return NotFound($"Collection with ID {request.CollectionId} not found.");
            }
            return Ok("Products removed from collection successfully.");
        }
    }
}
