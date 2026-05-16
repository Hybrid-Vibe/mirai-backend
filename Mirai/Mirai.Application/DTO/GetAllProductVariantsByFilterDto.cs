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
    }
}
