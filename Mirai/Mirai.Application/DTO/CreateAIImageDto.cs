using System.ComponentModel.DataAnnotations;

namespace Mirai.Application.DTO;

public class CreateAIImageDto
{
    [Required(ErrorMessage = "Prompt is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Prompt must be between 10 and 1000 characters")]
    public string Prompt { get; set; } = null!;

    [StringLength(1000, ErrorMessage = "Negative prompt must not exceed 1000 characters")]
    public string? NegativePrompt { get; set; }

    [StringLength(50, ErrorMessage = "Style must not exceed 50 characters")]
    public string? Style { get; set; }

    [Range(64, 2048, ErrorMessage = "Width must be between 64 and 2048")]
    public int? Width { get; set; }

    [Range(64, 2048, ErrorMessage = "Height must be between 64 and 2048")]
    public int? Height { get; set; }
}
