using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class GetAllProductsByFilterDto
    {
        public string ProductId { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandId { get; set; }
        public string? BrandName { get; set; }
        public decimal? RatingAvg { get; set; }

        public int RatingCount { get; set; }
        public List<GetAllProductVariantsByFilterDto> Variants { get; set; }
        public List<ProductImageDto> ProductImages { get; set; }
    }
}
