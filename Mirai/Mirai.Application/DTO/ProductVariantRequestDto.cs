using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class ProductVariantRequestDto
    {
        public string? Color { get; set; }

        public string? PhoneModel { get; set; }

        public decimal Price { get; set; }

        //public int Stock { get; set; }
        public string? ImageUrl { get; set; }
    }
}
