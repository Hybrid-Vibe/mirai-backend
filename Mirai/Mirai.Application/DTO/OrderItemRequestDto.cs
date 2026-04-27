using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class OrderItemRequestDto
    {
        public string? VariantId { get; set; }
        public int Quantity { get; set; }
    }
}
