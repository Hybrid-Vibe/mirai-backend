using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;

namespace Mirai.Infastructure.Services;

public class CollectionService : ICollectionService
{
    private readonly AppDbContext _context;

    public CollectionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CollectionResponseDto>> GetAllActiveAsync()
    {
        return await _context.Collections
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CollectionResponseDto
            {
                CollectionId = c.CollectionId,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                CoverImageUrl = c.CoverImageUrl,
                Tag = c.Tag,
                ItemCount = c.ProductCollections.Count()
            })
            .ToListAsync();
    }

    public async Task<CollectionResponseDto> GetBySlugAsync(string slug)
    {
        var collection = await _context.Collections
            .Include(c => c.ProductCollections)
            .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower() && c.IsActive);

        if (collection == null) return null;

        return new CollectionResponseDto
        {
            CollectionId = collection.CollectionId,
            Name = collection.Name,
            Slug = collection.Slug,
            Description = collection.Description,
            CoverImageUrl = collection.CoverImageUrl,
            Tag = collection.Tag,
            ItemCount = collection.ProductCollections.Count()
        };
    }

    public async Task<CollectionResponseDto> CreateCollectionAsync(CreateCollectionDto createDto)
    {
        var newCollection = new Collection
        {
            Name = createDto.Name,
            Slug = createDto.Slug,
            Description = createDto.Description,
            CoverImageUrl = createDto.CoverImageUrl,
            Tag = createDto.Tag,
            DisplayOrder = createDto.DisplayOrder
        };

        _context.Collections.Add(newCollection);
        await _context.SaveChangesAsync();

        return new CollectionResponseDto
        {
            CollectionId = newCollection.CollectionId,
            Name = newCollection.Name,
            Slug = newCollection.Slug,
            Description = newCollection.Description,
            CoverImageUrl = newCollection.CoverImageUrl,
            Tag = newCollection.Tag,
            ItemCount = 0
        };
    }

    public async Task<CollectionResponseDto> UpdateCollectionAsync(Guid id, UpdateCollectionDto updateDto)
    {
        var collection = await _context.Collections
            .Include(c => c.ProductCollections)
            .FirstOrDefaultAsync(c => c.CollectionId == id);

        if (collection == null) return null;

        collection.Name = updateDto.Name;
        collection.Slug = updateDto.Slug;
        collection.Description = updateDto.Description;
        collection.CoverImageUrl = updateDto.CoverImageUrl;
        collection.Tag = updateDto.Tag;
        collection.IsActive = updateDto.IsActive;
        collection.DisplayOrder = updateDto.DisplayOrder;
        collection.UpdatedAt = DateTime.UtcNow;

        _context.Collections.Update(collection);
        await _context.SaveChangesAsync();

        return new CollectionResponseDto
        {
            CollectionId = collection.CollectionId,
            Name = collection.Name,
            Slug = collection.Slug,
            Description = collection.Description,
            CoverImageUrl = collection.CoverImageUrl,
            Tag = collection.Tag,
            ItemCount = collection.ProductCollections.Count()
        };
    }

    public async Task<bool> DeleteCollectionAsync(Guid id)
    {
        var collection = await _context.Collections.FindAsync(id);
        if (collection == null) return false;

        _context.Collections.Remove(collection);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddProductsToCollectionAsync(AddProductsToCollectionRequestDto request)
    {
        var collection = await _context.Collections.FindAsync(request.CollectionId);
        if (collection == null) return false;

        foreach (var productId in request.ProductIds)
        {
            if (!await _context.ProductCollections.AnyAsync(pc => pc.CollectionId == request.CollectionId && pc.ProductId == productId))
            {
                _context.ProductCollections.Add(new ProductCollection
                {
                    CollectionId = request.CollectionId,
                    ProductId = productId
                });
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveProductsFromCollectionAsync(RemoveProductsFromCollectionRequestDto request)
    {
        var collection = await _context.Collections.FindAsync(request.CollectionId);
        if (collection == null) return false;

        var productCollections = await _context.ProductCollections
            .Where(pc => pc.CollectionId == request.CollectionId && request.ProductIds.Contains(pc.ProductId))
            .ToListAsync();

        if (productCollections.Any())
        {
            _context.ProductCollections.RemoveRange(productCollections);
            await _context.SaveChangesAsync();
        }

        return true;
    }
}
