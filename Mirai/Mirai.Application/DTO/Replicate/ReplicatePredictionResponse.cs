using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mirai.Application.DTO.Re;

public class ReplicatePredictionResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("output")]
    public JsonElement? PredictionOutput { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    public string? GetFirstOutputUrl()
    {
        if (PredictionOutput == null || PredictionOutput.Value.ValueKind == JsonValueKind.Null)
            return null;

        if (PredictionOutput.Value.ValueKind == JsonValueKind.String)
            return PredictionOutput.Value.GetString();

        if (PredictionOutput.Value.ValueKind == JsonValueKind.Array && PredictionOutput.Value.GetArrayLength() > 0)
        {
            var first = PredictionOutput.Value[0];
            if (first.ValueKind == JsonValueKind.String)
                return first.GetString();
        }

        return null;
    }
}
