using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mirai.Application.DTO;
using Mirai.Application.DTO.Admin;
using Mirai.Application.Interfaces.Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Infastructure.Services;

public class ReplicateImageService : IReplicateImageService
{
    private readonly HttpClient _httpClient;
    private readonly ReplicateOptions _options;
    private readonly ILogger<ReplicateImageService> _logger;
    private readonly IPromptOptimizerService _promptOptimizer;
    private readonly ILanguageDetector _languageDetector;

    public ReplicateImageService(
        HttpClient httpClient,
        IOptions<ReplicateOptions> options,
        ILogger<ReplicateImageService> logger, IPromptOptimizerService promptOptimizerService, ILanguageDetector languageDetector)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _promptOptimizer = promptOptimizerService;
        _languageDetector = languageDetector;
    }

    public static bool IsVietnamese(string text)
    {
        string vietnameseChars =
            "ăâđêôơưáàảãạấầẩẫậắằẳẵặéèẻẽẹíìỉĩịóòỏõọốồổỗộớờởỡợúùủũụứừửữựýỳỷỹỵ";

        return text
            .ToLower()
            .Any(c => vietnameseChars.Contains(c));
    }

    public async Task<ReplicateImageResult> GenerateAsync(
        CreateAIImageDto request,
        CancellationToken cancellationToken = default)
    {

        string finalPrompt;


        if (IsVietnamese(request.Prompt))
        {
            finalPrompt =
                await _promptOptimizer.OptimizeAsync(
                    request.Prompt,
                    cancellationToken);
        }
        else
        {
            finalPrompt = request.Prompt;
        }

        var body = new
        {
            input = new
            {
                prompt = finalPrompt,
                aspect_ratio = "3:4"
            }
        };

        _logger.LogInformation("Calling FLUX Schnell...");

        var response = await _httpClient.PostAsJsonAsync(
            $"models/{_options.Model}/predictions",
            body,
            cancellationToken
        );

        var raw = await response.Content.ReadAsStringAsync();


        _logger.LogInformation(
            "Replicate response: {raw}",
            raw
        );


        response.EnsureSuccessStatusCode();

        var prediction = JsonSerializer.Deserialize<PredictionResponse>(raw);

        if (prediction == null)
            throw new Exception("Invalid Replicate response");

        // polling
        while (prediction.status != "succeeded" &&
               prediction.status != "failed")
        {
            await Task.Delay(1500, cancellationToken);

            using var client = new HttpClient();


            client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _options.ApiToken
            );


            prediction =
            await client.GetFromJsonAsync<PredictionResponse>(
                prediction.urls.get,
                cancellationToken
            );
        }

        if (prediction.status == "failed")
            throw new Exception("FLUX generation failed");

        return new ReplicateImageResult
        {
            PredictionId = prediction.id,
            TemporaryImageUrl = ExtractOutput(prediction.output)
        };
    }

    // =========================
    // PROMPT BUILDER
    // =========================


    public static string BuildPrompt(string userPrompt)
    {
        return $@"
{userPrompt},
masterpiece, best quality, ultra detailed,
cinematic lighting, volumetric light,
sharp focus, realistic textures,
professional photography,
8k resolution, depth of field,
natural colors, highly detailed";
    }

    private static string ExtractOutput(JsonElement output)
    {
        if (output.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in output.EnumerateArray())
                return item.GetString() ?? "";
        }

        return output.GetString() ?? "";
    }

    // =========================
    // RESPONSE MODEL
    // =========================
    private class PredictionResponse
    {
        public string id { get; set; } = "";
        public string status { get; set; } = "";
        public JsonElement output { get; set; }
        public Urls urls { get; set; } = new();

        public class Urls
        {
            public string get { get; set; } = "";
        }
    }
}
