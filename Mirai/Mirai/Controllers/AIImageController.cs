using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Common.Exceptions;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Controllers;

[Route("api/ai-images")]
[ApiController]
[Authorize]
public class AIImageController : ControllerBase
{
    private readonly IAIImageService _aiImageService;

    public AIImageController(IAIImageService aiImageService)
    {
        _aiImageService = aiImageService;
    }

    private string GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedException("User not authenticated");
        }
        return userId;
    }

    [HttpPost]
    public async Task<ActionResult<AIImageDto>> CreateAIImage([FromBody] CreateAIImageDto request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _aiImageService.CreateAIImageAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetAIImageById), new { id = result.AIImageId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AIImageDto>> GetAIImageById(string id, CancellationToken cancellationToken)
    {
        var result = await _aiImageService.GetAIImageByIdAsync(id, cancellationToken);
        if (result == null)
        {
            throw new NotFoundException(nameof(AiImage), id);
        }
        return Ok(result);
    }

    [HttpGet("user")]
    public async Task<ActionResult<List<AIImageDto>>> GetUserAIImages(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _aiImageService.GetAIImagesByUserIdAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<AIImageDto>> UpdateAIImageStatus(string id, [FromBody] UpdateAIImageStatusDto request, CancellationToken cancellationToken)
    {
        var result = await _aiImageService.UpdateAIImageStatusAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAIImage(string id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _aiImageService.DeleteAIImageAsync(id, userId, cancellationToken);
        return NoContent();
    }
}
