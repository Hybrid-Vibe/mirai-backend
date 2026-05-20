using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class UpdateProfileUserDto
    {
        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
