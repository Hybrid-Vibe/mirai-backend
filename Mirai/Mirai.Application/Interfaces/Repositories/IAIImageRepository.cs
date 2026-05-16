using Mirai.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Application.Interfaces.Repositories;

public interface IAIImageRepository
{
    Task<AIImage?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<AIImage>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<AIImage> AddAsync(AIImage aiImage, CancellationToken cancellationToken = default);
    Task UpdateAsync(AIImage aiImage, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
