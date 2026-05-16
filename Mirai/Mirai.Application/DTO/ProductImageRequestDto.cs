using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class ProductImageRequestDto
    {
        public string? ImageUrl { get; set; }

        public bool IsPrimary { get; set; }
    }
}
