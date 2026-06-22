using Mirai.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IReplicateImageService
    {
        Task<ReplicateImageResult> GenerateAsync(
        CreateAIImageDto request,
        CancellationToken cancellationToken = default
    );
    }
}
