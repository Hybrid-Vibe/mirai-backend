using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class UpdateProductStar
    {
        public decimal? RatingAvg { get; set; }

        public int RatingCount { get; set; }
    }
}
