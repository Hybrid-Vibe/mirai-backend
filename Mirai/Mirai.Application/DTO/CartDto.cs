using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CartDto
    {
        public string? CartId { get; set; }
        public List<CartItemDto>? Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
