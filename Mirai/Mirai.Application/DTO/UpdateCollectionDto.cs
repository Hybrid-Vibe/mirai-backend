using System;

namespace Mirai.Application.DTO;

public class UpdateCollectionDto
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Tag { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
