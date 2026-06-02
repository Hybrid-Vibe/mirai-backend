using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class UpdatePaymentStatusResponse
    {
        public string PaymentId { get; set; } = null!;

        public PaymentStatusInPayment Status { get; set; }

        public decimal? Amount { get; set; }
    }
}
