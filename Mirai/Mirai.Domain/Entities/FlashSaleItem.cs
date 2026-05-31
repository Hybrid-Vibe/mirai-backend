using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class FlashSaleItem
{
    public string FlashSaleItemId { get; set; } = null!;

    public string FlashSaleId { get; set; } = null!;

    public string VariantId { get; set; } = null!;

    public decimal SalePrice { get; set; }

    public int QuantityLimit { get; set; }

    public int QuantitySold { get; set; }

    public int? PerUserLimit { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual FlashSale FlashSale { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ProductVariant Variant { get; set; } = null!;
}
