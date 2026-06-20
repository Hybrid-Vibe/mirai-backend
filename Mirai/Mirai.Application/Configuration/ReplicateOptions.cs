namespace Mirai.Application.Configuration;

public class ReplicateOptions
{
    public const string SectionName = "Replicate";

    public string ApiToken { get; set; } = string.Empty;

    public string Model { get; set; } = "black-forest-labs/flux-schnell";

    public int WaitTimeoutSeconds { get; set; } = 120;

    public int PollIntervalMs { get; set; } = 2000;

    public int DefaultInferenceSteps { get; set; } = 4;

    public bool GoFast { get; set; } = true;

    public string Format { get; set; } = "webp";

    public int Quality { get; set; } = 80;
}
