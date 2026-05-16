using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.ValidateOrderStatus
{
    public class OrderStateValidator
    {
        public static bool CanUpdateOrderStatus(OrderStatus current, OrderStatus next)
        {
            return current switch
            {
                OrderStatus.Created => next == OrderStatus.Confirmed || next == OrderStatus.Cancelled,
                OrderStatus.Confirmed => next == OrderStatus.Shipped || next == OrderStatus.Cancelled,
                OrderStatus.Shipped => next == OrderStatus.Delivered,
                OrderStatus.Delivered => false,
                OrderStatus.Cancelled => false,
                _ => false
            };
        }

        public static bool CanUpdatePaymentStatus(PaymentStatus current, PaymentStatus next)
        {
            return current switch
            {
                PaymentStatus.Unpaid => next == PaymentStatus.Paid || next == PaymentStatus.Failed,
                PaymentStatus.Paid => next == PaymentStatus.Refunded,
                PaymentStatus.Failed => false,
                PaymentStatus.Refunded => false,
                _ => false
            };
        }
    }
}
