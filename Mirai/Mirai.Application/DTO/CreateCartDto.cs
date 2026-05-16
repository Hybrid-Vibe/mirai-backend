using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CreateCartDto
    {
        public string UserId { get; set; } = null!;
        public string VariantId { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
