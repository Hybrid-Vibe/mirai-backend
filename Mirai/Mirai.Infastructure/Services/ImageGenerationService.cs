using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai.Application.Configuration;
using Mirai.Application.DTO;
using Mirai.Application.DTO.Re;
using Mirai.Application.Interfaces.Services;

namespace Mirai.Infastructure.Services;

public class ImageGenerationService : IImageGenerationService
{
    private static readonly HashSet<string> TerminalStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "succeeded",
        "failed",
        "canceled"
    };

    private static readonly HashSet<string> InProgressStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "starting",
        "processing"
    };

    private readonly IReplicateClient _replicateClient;
    private readonly ReplicateOptions _options;
    private readonly ILogger<ImageGenerationService> _logger;

    public ImageGenerationService(
        IReplicateClient replicateClient,
        IOptions<ReplicateOptions> options,
        ILogger<ImageGenerationService> logger)
    {
        _replicateClient = replicateClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ImageGenerationResult> GenerateAsync(
        ImageGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prediction = await _replicateClient.CreatePredictionAsync(request, cancellationToken);

            if (InProgressStatuses.Contains(prediction.Status ?? string.Empty))
            {
                _logger.LogInformation(
                    "Prediction {PredictionId} still in progress, polling...",
                    prediction.Id);

                prediction = await WaitForCompletionAsync(prediction.Id!, cancellationToken);
            }

            return MapPrediction(prediction);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Image generation failed");
            return new ImageGenerationResult
            {
                Success = false,
                Status = "failed",
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<ImageGenerationResult> GetPredictionStatusAsync(
        string predictionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prediction = await _replicateClient.GetPredictionAsync(predictionId, cancellationToken);
            return MapPrediction(prediction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Replicate prediction {PredictionId}", predictionId);
            return new ImageGenerationResult
            {
                Success = false,
                PredictionId = predictionId,
                Status = "failed",
                ErrorMessage = ex.Message
            };
        }
    }

    private async Task<ReplicatePredictionResponse> WaitForCompletionAsync(
        string predictionId,
        CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow.AddSeconds(_options.WaitTimeoutSeconds);

        while (DateTime.UtcNow < deadline)
        {
            await Task.Delay(_options.PollIntervalMs, cancellationToken);

            var prediction = await _replicateClient.GetPredictionAsync(predictionId, cancellationToken);

            if (TerminalStatuses.Contains(prediction.Status ?? string.Empty))
                return prediction;
        }

        throw new TimeoutException(
            $"Replicate prediction {predictionId} did not complete within {_options.WaitTimeoutSeconds} seconds");
    }

    private static ImageGenerationResult MapPrediction(ReplicatePredictionResponse prediction)
    {
        var succeeded = string.Equals(prediction.Status, "succeeded", StringComparison.OrdinalIgnoreCase);
        var imageUrl = prediction.GetFirstOutputUrl();

        if (succeeded && string.IsNullOrWhiteSpace(imageUrl))
        {
            return new ImageGenerationResult
            {
                Success = false,
                PredictionId = prediction.Id,
                Status = prediction.Status,
                ErrorMessage = "Replicate returned success but no image URL was found in the response"
            };
        }

        return new ImageGenerationResult
        {
            Success = succeeded,
            PredictionId = prediction.Id,
            Status = prediction.Status,
            ImageUrl = imageUrl,
            ThumbnailUrl = imageUrl,
            ErrorMessage = succeeded ? null : prediction.Error ?? $"Prediction status: {prediction.Status}"
        };
    }
}
