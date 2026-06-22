using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class PhoneCaseTemplate
    {
        public string Model { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string AspectRatio { get; set; } = "3:4";
        public string CameraHint { get; set; } = null!;
        public string SafeZoneHint { get; set; } = null!;
    }
}
