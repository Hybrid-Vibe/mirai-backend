using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? CategoryId { get; set; }

        public string? BrandId { get; set; }
    }
}
