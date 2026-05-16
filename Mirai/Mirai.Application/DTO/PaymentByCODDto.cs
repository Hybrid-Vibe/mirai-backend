using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class PaymentByCODDto
    {
        public string? OrderId { get; set; }

        public decimal? Amount { get; set; }
    }
}
