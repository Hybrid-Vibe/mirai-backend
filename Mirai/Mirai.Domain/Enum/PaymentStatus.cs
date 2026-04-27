using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Domain.Enum
{
    public enum PaymentStatus
    {
        Unpaid = 1,
        Paid = 2,
        Failed = 3,
        Refunded = 4
    }
}
