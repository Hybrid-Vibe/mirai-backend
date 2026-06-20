using System.Text.Json.Serialization;

namespace Mirai.Application.DTO.Re;

public class ReplicateCreatePredictionRequest
{
    [JsonPropertyName("input")]
    public ReplicateFluxInput Input { get; set; } = new();
}

public class ReplicateFluxInput
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = null!;

    [JsonPropertyName("go_fast")]
    public bool GoFast { get; set; }

    [JsonPropertyName("megapixels")]
    public string Megapixels { get; set; } = "1";

    [JsonPropertyName("num_outputs")]
    public int NumOutputs { get; set; } = 1;

    [JsonPropertyName("aspect_ratio")]
    public string AspectRatio { get; set; } = "16:9";

    [JsonPropertyName("output_format")]
    public string Format { get; set; } = "webp";

    [JsonPropertyName("output_quality")]
    public int Quality { get; set; } = 80;

    [JsonPropertyName("num_inference_steps")]
    public int NumInferenceSteps { get; set; } = 4;
}
