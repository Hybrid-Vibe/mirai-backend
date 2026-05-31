using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class CreateReviewRequest
    {
        public string UserId { get; set; } = null!;

        public string ProductId { get; set; } = null!;

        public string? VariantId { get; set; }

        public int Rating { get; set; }

        public string? Title { get; set; }

        public string? Comment { get; set; }
    }
}
