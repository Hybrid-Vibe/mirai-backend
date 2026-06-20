using Mirai.Application.DTO;
using Mirai.Application.DTO.Re;

namespace Mirai.Application.Interfaces.Services;

public interface IReplicateClient
{
    Task<ReplicatePredictionResponse> CreatePredictionAsync(ImageGenerationRequest request, CancellationToken cancellationToken = default);

    Task<ReplicatePredictionResponse> GetPredictionAsync(string predictionId, CancellationToken cancellationToken = default);
}
