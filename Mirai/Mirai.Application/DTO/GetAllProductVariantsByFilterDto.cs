using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class GetAllProductVariantsByFilterDto
    {
        public string? VariantId { get; set; }
        public string? Color { get; set; }
        public string? PhoneModel { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public decimal? FlashSalePrice { get; set; }

        public bool IsFlashSale { get; set; }

        public DateTime? FlashSaleStartTime { get; set; }

        public DateTime? FlashSaleEndTime { get; set; }
    }
}
