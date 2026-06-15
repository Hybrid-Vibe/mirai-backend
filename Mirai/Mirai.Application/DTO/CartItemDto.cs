using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CartItemDto
    {
        public string? CartItemId { get; set; }
        public string? VariantId { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Image { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? Total { get; set; }
    }
}
