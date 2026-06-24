using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public class Collection
{
    public Guid CollectionId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Tag { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ProductCollection> ProductCollections { get; set; } = new List<ProductCollection>();
}
