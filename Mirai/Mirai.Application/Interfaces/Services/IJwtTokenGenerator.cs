using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
