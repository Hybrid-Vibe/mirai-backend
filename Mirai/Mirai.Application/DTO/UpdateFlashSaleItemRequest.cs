using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class UpdateFlashSaleItemRequest
    {
        public string? FlashSaleItemId { get; set; }

        public string VariantId { get; set; }

        public decimal SalePrice { get; set; }

        public int QuantityLimit { get; set; }

        public int PerUserLimit { get; set; }
    }
}
