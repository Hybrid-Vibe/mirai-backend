using Microsoft.EntityFrameworkCore;
using Mirai.Application.Interfaces.Repositories;
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

    public async Task<AiImage?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.AiImages
            .AsNoTracking()
            .FirstOrDefaultAsync(ai => ai.AiImageId == id, cancellationToken);
    }

    public async Task<List<AiImage>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.AiImages
            .AsNoTracking()
            .Where(ai => ai.UserId == userId)
            .OrderByDescending(ai => ai.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<AiImage> AddAsync(AiImage aiImage, CancellationToken cancellationToken = default)
    {
        await _context.AiImages.AddAsync(aiImage, cancellationToken);
        return aiImage;
    }

    public Task UpdateAsync(AiImage aiImage, CancellationToken cancellationToken = default)
    {
        _context.AiImages.Update(aiImage);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteAsync(string id, string userId, CancellationToken cancellationToken = default)
    {
        var aiImage = await _context.AiImages
            .FirstOrDefaultAsync(ai => ai.AiImageId == id && ai.UserId == userId, cancellationToken);

        if (aiImage == null)
            return false;

        _context.AiImages.Remove(aiImage);
        return true;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.AiImages
            .AnyAsync(ai => ai.AiImageId == id, cancellationToken);
    }
}
