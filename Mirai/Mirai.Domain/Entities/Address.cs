using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class Address
{
    public string AddressId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? RecipientName { get; set; }

    public string? RecipientPhone { get; set; }

    public string AddressLine { get; set; } = null!;

    public string? Ward { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public string? Province { get; set; }

    public string? PostalCode { get; set; }

    public string? Note { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();

    public virtual User User { get; set; } = null!;
}
