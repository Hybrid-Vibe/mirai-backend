namespace Mirai.Application.DTO;

public class ImageGenerationRequest
{
    public string Prompt { get; set; } = null!;

    public string? NegativePrompt { get; set; }

    public string? Style { get; set; }

    public int Width { get; set; } = 1024;

    public int Height { get; set; } = 576;
}
