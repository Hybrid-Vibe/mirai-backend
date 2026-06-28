using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class SaveGeneratedImageDto
    {
        public string Prompt { get; set; } = "";
        public string? NegativePrompt { get; set; }
        public string? Style { get; set; }
        public string ImageUrl { get; set; } = "";
        public string? PredictionId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
