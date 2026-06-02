using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class OrderItemsDto
    {
        public string OrderId { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string? OrderNumber { get; set; }

        public decimal? Subtotal { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? ShippingFee { get; set; }

        public decimal? TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string? Currency { get; set; }

        public int? Status { get; set; }

        public int? PaymentStatus { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
