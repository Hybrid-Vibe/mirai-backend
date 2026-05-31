using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class GetFlashSaleProductsDto
    {
        public string ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? VariantId { get; set; }

        public string? Color { get; set; }

        public string? PhoneModel { get; set; }
        public decimal OriginalPrice { get; set; }

        public decimal FlashSalePrice { get; set; }

        public int Stock { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
