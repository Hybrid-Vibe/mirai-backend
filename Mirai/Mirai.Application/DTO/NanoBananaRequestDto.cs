using System.Collections.Generic;

namespace Mirai.Application.DTO;

public class NanoBananaRequestDto
{
    public string Prompt { get; set; } = null!;

    public string? NegativePrompt { get; set; }

    public string? Style { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? Steps { get; set; } = 20;

    public float? CfgScale { get; set; } = 7.5f;

    public string? Seed { get; set; }

    public List<string>? Sampler { get; set; }
}
