using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class PaymentDto
    {
        public string? OrderId { get; set; }

        public decimal? Amount { get; set; }

        public string? TransactionId { get; set; }

    }
}
