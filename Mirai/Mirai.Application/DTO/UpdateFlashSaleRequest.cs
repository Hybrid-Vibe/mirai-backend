using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class UpdateFlashSaleRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<UpdateFlashSaleItemRequest> Items { get; set; }
    }
}
