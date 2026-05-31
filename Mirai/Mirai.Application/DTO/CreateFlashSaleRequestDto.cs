using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CreateFlashSaleRequestDto
    {
            public string Title { get; set; }

            public string? Description { get; set; }

            public DateTime StartTime { get; set; }

            public DateTime EndTime { get; set; }

            public List<CreateFlashSaleItemRequest>? Items { get; set; }
        
    }
}
