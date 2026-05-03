using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Extension
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        //public List<string> Colors { get; set; }
    }
}
