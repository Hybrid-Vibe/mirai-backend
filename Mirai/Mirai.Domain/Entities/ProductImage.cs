using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class ProductImage
{
    public string ImageId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string? VariantId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ProductVariant? Variant { get; set; }
}
