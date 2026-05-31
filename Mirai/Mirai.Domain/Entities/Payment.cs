using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public int? Method { get; set; }

    public int? Provider { get; set; }

    public int? Status { get; set; }

    public decimal? Amount { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? FailureReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
