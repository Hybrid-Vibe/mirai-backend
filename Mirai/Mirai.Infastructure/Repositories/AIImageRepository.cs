using Microsoft.EntityFrameworkCore;
using Mirai.Application.Extension;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.SearchFilter.Admin;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Infastructure.Repositories;

public class AIImageRepository : IAIImageRepository
{
    private readonly AppDbContext _context;

    public AIImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AIImage?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.AIImages
            .AsNoTracking()
            .FirstOrDefaultAsync(ai => ai.AIImageId == id, cancellationToken);
    }

    public async Task<List<AIImage>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.AIImages
            .AsNoTracking()
            .Where(ai => ai.UserId == userId)
            .OrderByDescending(ai => ai.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<AIImage> AddAsync(AIImage aiImage, CancellationToken cancellationToken = default)
    {
        await _context.AIImages.AddAsync(aiImage, cancellationToken);
        return aiImage;
    }

    public Task UpdateAsync(AIImage aiImage, CancellationToken cancellationToken = default)
    {
        _context.AIImages.Update(aiImage);
        return Task.CompletedTask;
    }

    public async Task<PagedResult<AIImage>> GetAllPagedAsync(AdminAIImageFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.AIImages.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(ai => ai.UserId == filter.UserId);

        if (filter.Status.HasValue)
            query = query.Where(ai => ai.Status == filter.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(ai => ai.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AIImage>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<bool> DeleteAsync(string id, string userId, CancellationToken cancellationToken = default)
    {
        var aiImage = await _context.AIImages
            .FirstOrDefaultAsync(ai => ai.AIImageId == id && ai.UserId == userId, cancellationToken);

        if (aiImage == null)
            return false;

        _context.AIImages.Remove(aiImage);
        return true;
    }

    public async Task<bool> AdminDeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var aiImage = await _context.AIImages
            .FirstOrDefaultAsync(ai => ai.AIImageId == id, cancellationToken);

        if (aiImage == null)
            return false;

        _context.AIImages.Remove(aiImage);
        return true;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.AIImages
            .AnyAsync(ai => ai.AIImageId == id, cancellationToken);
    }
}
