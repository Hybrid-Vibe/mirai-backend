using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class OrderItem
{
    public string OrderItemId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public string VariantId { get; set; } = null!;

    public int Quantity { get; set; }

    public string? ProductName { get; set; }

    public string? VariantName { get; set; }

    public decimal? Price { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? DiscountAmount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ProductVariant Variant { get; set; } = null!;
}
