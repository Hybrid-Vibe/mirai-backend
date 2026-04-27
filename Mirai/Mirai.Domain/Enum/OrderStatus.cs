using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Domain.Enum
{
    public enum OrderStatus
    {
        Created = 1,     
        Confirmed = 2,   
        Shipped = 3,     
        Delivered = 4,   
        Cancelled = 5   
    }
}
