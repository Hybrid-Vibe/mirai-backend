using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.SearchFilter
{
    public class CartSearchFilter
    {
        public string? UserId { get; set; }
        public string? CartId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
