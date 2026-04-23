using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CreateProductImageDto
    {
        public string ProductId { get; set; } = null!;

        public string? VariantId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string? AltText { get; set; }
    }
}
