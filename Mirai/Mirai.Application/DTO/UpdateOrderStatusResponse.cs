using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class UpdateOrderStatusResponse
    {
        public string OrderId { get; set; }

        public OrderStatus Status { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
