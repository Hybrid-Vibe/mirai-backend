using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class CartItem
{
    public string CartItemId { get; set; } = null!;

    public string CartId { get; set; } = null!;

    public string VariantId { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CustomImageUrl { get; set; }

    public string? CustomDesignConfig { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ProductVariant Variant { get; set; } = null!;
}
