using Mirai.Domain.Enums;

namespace Mirai.Domain.Entities;

public partial class AIImage
{
    public string AIImageId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? Prompt { get; set; }

    public string? NegativePrompt { get; set; }

    public string? ImageUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? Style { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public AIImageStatus Status { get; set; }

    public string? ErrorMessage { get; set; }

    public string? NanoBananaRequestId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
