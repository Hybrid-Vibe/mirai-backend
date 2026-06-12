using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class PayOSWebhookRootDto
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public bool Success { get; set; }
        public PayOSWebhookDto Data { get; set; }
        public string Signature { get; set; }
    }
}
