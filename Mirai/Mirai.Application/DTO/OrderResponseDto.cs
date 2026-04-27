using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class OrderResponseDto
    {
        public string? OrderId { get; set; }
        public string? OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CreatedAt { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
