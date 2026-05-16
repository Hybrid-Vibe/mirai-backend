using Mirai.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mirai.Application.DTO;

public class UpdateAIImageStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    public AIImageStatus Status { get; set; }

    [Url(ErrorMessage = "Image URL must be a valid URL")]
    public string? ImageUrl { get; set; }

    [Url(ErrorMessage = "Thumbnail URL must be a valid URL")]
    public string? ThumbnailUrl { get; set; }

    [StringLength(500, ErrorMessage = "Error message must not exceed 500 characters")]
    public string? ErrorMessage { get; set; }
}
