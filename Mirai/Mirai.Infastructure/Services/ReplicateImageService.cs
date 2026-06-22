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

    public ReplicateImageService(
        HttpClient httpClient,
        IOptions<ReplicateOptions> options,
        ILogger<ReplicateImageService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ReplicateImageResult> GenerateAsync(
        CreateAIImageDto request,
        CancellationToken cancellationToken = default)
    {

        var body = new
        {
            input = new
            {
                prompt = BuildPrompt(
            request.Prompt,
            PhoneCaseTemplates.Map["iphone-14-pro-max"]
        ),

                aspect_ratio = "3:4",

                num_outputs = 1,

                output_format = "png",

                go_fast = true
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
    public static string BuildPrompt(
    string userPrompt,
    PhoneCaseTemplate model)
    {

        return $@"

Create a realistic flat back view phone case design.


The final image should look like a real physical phone case sample,
not a phone and not a technical drawing.


PRODUCT:
{model.Model} protective case.


CASE SHAPE:
- exact iPhone case silhouette
- vertical rounded rectangle
- curved corners
- slim transparent protective edge
- back side view only


CAMERA DESIGN:
Top-left camera area:

{model.CameraHint}


Camera details:
- realistic camera protection ring
- raised border around camera cutout
- empty camera opening
- correct iPhone 14 Pro Max camera position
- no extra camera holes


ARTWORK:
{userPrompt}


DESIGN:
- premium phone case artwork
- full back surface coverage
- clean composition
- suitable for UV printing


BACKGROUND:
- isolated phone case
- white background
- soft studio presentation
- if dark mode, use dark neutral background


STRICT RULES:
- only one phone case
- back side only
- no smartphone inside
- no screen
- no front phone
- no hand
- no person
- no text
- no logo
- no watermark
- no measurement labels
- no dimensions
- no technical drawing


OUTPUT:
flat centered phone case,
real product shape,
high resolution

";

    }

    // =========================
    // WIDTH/HEIGHT → ASPECT RATIO
    // =========================
    private static string GetAspectRatio(int? width, int? height)
    {
        if (!width.HasValue || !height.HasValue)
            return "3:4";

        var w = width.Value;
        var h = height.Value;

        return (w, h) switch
        {
            (512, 512) => "1:1",
            (768, 1024) => "3:4",
            (1024, 768) => "4:3",
            (1080, 1920) => "9:16",
            (1920, 1080) => "16:9",
            _ => "3:4"
        };
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
