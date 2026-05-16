using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class PaymentResponseModel
    {
        public string? PaymentMethod { get; set; } 
        public string? OrderDescription { get; set; }
        public string? OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? VnPayResponseCode { get; set; }
        public string? Message { get; set; }
    }
}
