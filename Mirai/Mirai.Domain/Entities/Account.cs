using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string Provider { get; set; } = null!;

    public string ProviderAccountId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
