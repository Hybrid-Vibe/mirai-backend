using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string Role { get; set; } = null!;
    }
}
