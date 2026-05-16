using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CreateProductRequestDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? CategoryId { get; set; }

        public string? BrandId { get; set; }

        public List<ProductImageRequestDto> Images { get; set; } = [];

        public List<ProductVariantRequestDto> Variants { get; set; } = [];
    }
}
