using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class Shipping
{
    public string ShippingId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public string AddressId { get; set; } = null!;

    public int? ShippingStatus { get; set; }

    public string? Carrier { get; set; }

    public string? TrackingCode { get; set; }

    public decimal? ShippingFee { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
