using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
        Task BlacklistTokenAsync(string token);

        Task<bool> IsBlacklistedAsync(string token);
    }
}
