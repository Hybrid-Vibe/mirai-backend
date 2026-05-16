using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.ValidateOrderStatus
{
    public class PaymentStateValidator
    {
        public static bool CanUpdatePaymentStatus(PaymentStatusInPayment current, PaymentStatusInPayment next)
        {
            return current switch
            {
                PaymentStatusInPayment.Pending => next == PaymentStatusInPayment.Succeed || next == PaymentStatusInPayment.Failed,

                PaymentStatusInPayment.Succeed => next == PaymentStatusInPayment.Refunded,

                PaymentStatusInPayment.Failed => false, 

                PaymentStatusInPayment.Refunded => false, 

                _ => false
            };
        }
    }
}
