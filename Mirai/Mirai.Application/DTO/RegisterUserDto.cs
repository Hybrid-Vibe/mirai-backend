using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.DTO
{
    public class RegisterUserDto
    {
        public string? FullName { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Phone { get; set; }
    }
}
