using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CreateProductVariantDto
    {
        public string ProductId { get; set; } = null!;

        public string? Color { get; set; }

        public string? PhoneModel { get; set; }

        public decimal? Price { get; set; }

        public decimal? CompareAtPrice { get; set; }

        public decimal? CostPrice { get; set; }

        //public string? Barcode { get; set; }

        public int? Stock { get; set; }
    }
}
