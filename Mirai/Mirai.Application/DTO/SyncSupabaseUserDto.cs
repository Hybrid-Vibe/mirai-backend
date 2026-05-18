using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Mirai.Application.DTO
{
    public class SyncSupabaseUserDto
    {
        public string SupabaseUid { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string? FullName { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
