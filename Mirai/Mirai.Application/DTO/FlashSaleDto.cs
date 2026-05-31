using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class FlashSaleDto
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        public string FlashSaleId { get; set; } = null!;

        public string VariantId { get; set; } = null!;

        public decimal SalePrice { get; set; }

        public int QuantityLimit { get; set; }

        public int QuantitySold { get; set; }

        public int? PerUserLimit { get; set; }
    }
}
