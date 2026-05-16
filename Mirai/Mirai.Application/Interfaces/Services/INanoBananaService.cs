using Mirai.Application.DTO;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Application.Interfaces.Services;

public interface INanoBananaService
{
    Task<NanoBananaResponseDto> GenerateImageAsync(NanoBananaRequestDto request, CancellationToken cancellationToken = default);
    Task<NanoBananaResponseDto> GetImageStatusAsync(string requestId, CancellationToken cancellationToken = default);
}
