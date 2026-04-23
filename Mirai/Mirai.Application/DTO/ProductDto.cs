using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class ProductDto
    {
        public string ProductId { get; set; } = null!;

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public string? BrandId { get; set; }
        public string? BrandName { get; set; } 

        public bool IsActive { get; set; }

        public decimal? RatingAvg { get; set; }

        public int RatingCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
