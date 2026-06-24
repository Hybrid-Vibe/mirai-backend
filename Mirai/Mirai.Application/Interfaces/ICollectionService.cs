using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirai.Application.DTO;

namespace Mirai.Application.Interfaces;

public interface ICollectionService
{
    Task<IEnumerable<CollectionResponseDto>> GetAllActiveAsync();
    Task<CollectionResponseDto> GetBySlugAsync(string slug);
    Task<CollectionResponseDto> CreateCollectionAsync(CreateCollectionDto createDto);
    Task<CollectionResponseDto> UpdateCollectionAsync(Guid id, UpdateCollectionDto updateDto);
    Task<bool> DeleteCollectionAsync(Guid id);
    Task<bool> AddProductsToCollectionAsync(AddProductsToCollectionRequestDto request);
    Task<bool> RemoveProductsFromCollectionAsync(RemoveProductsFromCollectionRequestDto request);
}
