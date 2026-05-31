using Mirai.Application.Extension;
using Mirai.Application.SearchFilter.Admin;
using Mirai.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Application.Interfaces.Repositories;

public interface IAIImageRepository
{
    Task<AiImage?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<AiImage>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<PagedResult<AiImage>> GetAllPagedAsync(AdminAIImageFilter filter, CancellationToken cancellationToken = default);
    Task<AiImage> AddAsync(AiImage aiImage, CancellationToken cancellationToken = default);
    Task UpdateAsync(AiImage aiImage, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, string userId, CancellationToken cancellationToken = default);
    Task<bool> AdminDeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
