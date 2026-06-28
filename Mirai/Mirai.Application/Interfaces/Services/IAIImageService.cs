using Mirai.Application.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Application.Interfaces.Services;

public interface IAIImageService
{
    Task<GenerateAIImageResultDto> CreateAIImageAsync(
    string userId,
    CreateAIImageDto createDto,
    CancellationToken cancellationToken = default);

    Task<AIImageDto> SaveGeneratedAsync(
        string userId,
        SaveGeneratedImageDto dto,
        CancellationToken cancellationToken = default);
    Task<AIImageDto?> GetAIImageByIdAsync(string aiImageId, CancellationToken cancellationToken = default);
    Task<List<AIImageDto>> GetAIImagesByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<AIImageDto> UpdateAIImageStatusAsync(string aiImageId, UpdateAIImageStatusDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAIImageAsync(string aiImageId, string userId, CancellationToken cancellationToken = default);
}
