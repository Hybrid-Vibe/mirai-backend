using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai.Application.Configuration;
using Mirai.Application.DTO;
using Mirai.Application.DTO.Re;
using Mirai.Application.Interfaces.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Mirai.Infastructure.Clients;

public class ReplicateClient : IReplicateClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ReplicateOptions _options;
    private readonly ILogger<ReplicateClient> _logger;

    public ReplicateClient(HttpClient httpClient, IOptions<ReplicateOptions> options, ILogger<ReplicateClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ReplicatePredictionResponse> CreatePredictionAsync(
        ImageGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateConfiguration();

        var prompt = BuildPrompt(request);
        var payload = new ReplicateCreatePredictionRequest
        {
            Input = new ReplicateFluxInput
            {
                Prompt = prompt,
                GoFast = _options.GoFast,
                AspectRatio = ReplicateAspectRatioHelper.FromDimensions(request.Width, request.Height),
                Format = _options.Format,
                Quality = _options.Quality,
                NumInferenceSteps = _options.DefaultInferenceSteps
            }
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"v1/models/{_options.Model}/predictions");

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);
        httpRequest.Headers.TryAddWithoutValidation("Prefer", $"wait={_options.WaitTimeoutSeconds}");
        httpRequest.Content = JsonContent.Create(payload, options: JsonOptions);

        _logger.LogInformation(
            "Creating Replicate prediction for model {Model}, aspect {AspectRatio}",
            _options.Model,
            payload.Input.AspectRatio);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Replicate create prediction failed: {StatusCode} - {Body}",
                response.StatusCode,
                body);

            throw new InvalidOperationException($"Replicate API error ({(int)response.StatusCode}): {body}");
        }

        var prediction = JsonSerializer.Deserialize<ReplicatePredictionResponse>(body, JsonOptions)
            ?? throw new InvalidOperationException("Failed to parse Replicate prediction response");

        _logger.LogInformation(
            "Replicate prediction {PredictionId} status: {Status}",
            prediction.Id,
            prediction.Status);

        return prediction;
    }

    public async Task<ReplicatePredictionResponse> GetPredictionAsync(
        string predictionId,
        CancellationToken cancellationToken = default)
    {
        ValidateConfiguration();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"v1/predictions/{predictionId}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Replicate get prediction failed: {StatusCode} - {Body}",
                response.StatusCode,
                body);

            throw new InvalidOperationException($"Replicate API error ({(int)response.StatusCode}): {body}");
        }

        return JsonSerializer.Deserialize<ReplicatePredictionResponse>(body, JsonOptions)
            ?? throw new InvalidOperationException("Failed to parse Replicate prediction response");
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.ApiToken))
            throw new InvalidOperationException("Replicate:ApiToken is not configured");
    }

    private static string BuildPrompt(ImageGenerationRequest request)
    {
        var parts = new List<string> { request.Prompt.Trim() };

        if (!string.IsNullOrWhiteSpace(request.Style))
            parts.Add($"Style: {request.Style.Trim()}");

        // FLUX Schnell has no negative_prompt input; keep metadata in DB only.
        return string.Join(". ", parts);
    }
}
