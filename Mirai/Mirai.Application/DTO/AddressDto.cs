using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class AddressDto
    {
        public string UserId { get; set; } = null!;

        public string? RecipientName { get; set; }

        public string? RecipientPhone { get; set; }

        public string AddressLine { get; set; } = null!;

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? Province { get; set; }
        public string? Note { get; set; }
    }
}
