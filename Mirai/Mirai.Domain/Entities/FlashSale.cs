using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class FlashSale
{
    public string FlashSaleId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<FlashSaleItem> FlashSaleItems { get; set; } = new List<FlashSaleItem>();
}
