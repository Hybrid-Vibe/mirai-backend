using System;

namespace Mirai.Application.DTO;

public class CollectionResponseDto
{
    public Guid CollectionId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Tag { get; set; }
    public int ItemCount { get; set; }
}
