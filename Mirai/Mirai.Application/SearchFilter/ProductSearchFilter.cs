using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.SearchFilter
{
    public class ProductSearchFilter
    {

        public string? ProductId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandId { get; set; }
        public string? BrandName { get; set; }
        /*public string? VariantId { get; set; }
        public string? Color { get; set; }
        public string? PhoneModel { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }*/

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
