using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class PayOSWebhookDto
    {
        public long OrderCode { get; set; }
        public string? Status { get; set; }
        public decimal Amount { get; set; }
    }
}
