using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
