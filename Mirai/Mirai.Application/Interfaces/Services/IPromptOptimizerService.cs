using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IPromptOptimizerService
    {
        Task<string> OptimizeAsync(
            string prompt,
            CancellationToken cancellationToken);
    }
}
