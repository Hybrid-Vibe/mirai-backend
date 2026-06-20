using Mirai.Application.DTO;

namespace Mirai.Application.Interfaces.Services;

public interface IImageGenerationService
{
    Task<ImageGenerationResult> GenerateAsync(ImageGenerationRequest request, CancellationToken cancellationToken = default);

    Task<ImageGenerationResult> GetPredictionStatusAsync(string predictionId, CancellationToken cancellationToken = default);
}
