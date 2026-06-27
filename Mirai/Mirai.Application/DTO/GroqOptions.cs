using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class GroqOptions
    {
        public string ApiKey { get; set; } = "";
        public string BaseUrl { get; set; } = "";
        public string Model { get; set; } = "";
    }
}
