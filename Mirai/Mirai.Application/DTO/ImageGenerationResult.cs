namespace Mirai.Application.DTO;

public class ImageGenerationResult
{
    public bool Success { get; set; }

    public string? PredictionId { get; set; }

    public string? Status { get; set; }

    public string? ImageUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? ErrorMessage { get; set; }
}
