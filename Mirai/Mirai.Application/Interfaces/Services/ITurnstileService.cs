using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface ITurnstileService
    {
        Task<bool> VerifyAsync(string token);
    }
}
