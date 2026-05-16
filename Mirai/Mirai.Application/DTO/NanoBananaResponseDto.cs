using System.Collections.Generic;

namespace Mirai.Application.DTO;

public class NanoBananaResponseDto
{
    public bool Success { get; set; }

    public string? RequestId { get; set; }

    public string? Status { get; set; }

    public string? ImageUrl { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public Dictionary<string, object>? Metadata { get; set; }
}
