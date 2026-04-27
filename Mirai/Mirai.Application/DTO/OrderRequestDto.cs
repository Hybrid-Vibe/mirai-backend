using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class OrderRequestDto
    {
        public string UserId { get; set; }
        public string? Note { get; set; }
        public List<OrderItemRequestDto> Products { get; set; } = new();
    }
}
