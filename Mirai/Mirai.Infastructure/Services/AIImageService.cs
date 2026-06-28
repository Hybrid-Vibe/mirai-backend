using Microsoft.Extensions.Logging;
using Mirai.Application.Common.Exceptions;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using Mirai.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mirai.Infastructure.Services;

public class AIImageService : IAIImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReplicateImageService _replicateImageService;
    private readonly IStorageService _storageService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AIImageService> _logger;

    public AIImageService(
        IUnitOfWork unitOfWork,
        IReplicateImageService replicateImageService,
        IStorageService storageService,
        IHttpClientFactory httpClientFactory,
        ILogger<AIImageService> logger)
    {
        _unitOfWork = unitOfWork;
        _replicateImageService = replicateImageService;
        _storageService = storageService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<GenerateAIImageResultDto> CreateAIImageAsync(
    string userId,
    CreateAIImageDto createDto,
    CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generate AI image (no DB save) for user {UserId}", userId);

        var result = await _replicateImageService.GenerateAsync(
            createDto,
            cancellationToken
        );

        var httpClient = _httpClientFactory.CreateClient("ExternalMedia");

        var imageBytes = await httpClient.GetByteArrayAsync(
            result.TemporaryImageUrl,
            cancellationToken
        );

        var extension = Path.GetExtension(new Uri(result.TemporaryImageUrl).AbsolutePath)
                .TrimStart('.');

        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = "webp";
        }

        var permanentUrl = await _storageService.UploadImageByAI(
            imageBytes,
            userId,
            extension,
            cancellationToken
        );

        _logger.LogInformation("Replicate image generation success for user {UserId}", userId);

        return new GenerateAIImageResultDto
        {
            ImageUrl = permanentUrl,
            PredictionId = result.PredictionId,
            Prompt = createDto.Prompt,
            NegativePrompt = createDto.NegativePrompt,
            Style = createDto.Style,
            Width = createDto.Width ?? 512,
            Height = createDto.Height ?? 512
        };
    }

    public async Task<AIImageDto> SaveGeneratedAsync(
    string userId,
    SaveGeneratedImageDto dto,
    CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Saving generated AI image to DB for user {UserId}", userId);

        var aiImage = new AiImage
        {
            AiImageId = Guid.NewGuid().ToString(),
            UserId = userId,
            Prompt = dto.Prompt,
            NegativePrompt = dto.NegativePrompt,
            Style = dto.Style,
            ImageUrl = dto.ImageUrl,
            ThumbnailUrl = dto.ImageUrl,
            Width = dto.Width,
            Height = dto.Height,
            Status = (int)AIImageStatus.Completed,
            NanoBananaRequestId = dto.PredictionId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _unitOfWork.AIImageRepository.AddAsync(aiImage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AI image saved to DB {ImageId} for user {UserId}", aiImage.AiImageId, userId);

        return MapToDto(aiImage);
    }

    private static AIImageDto MapToDto(AiImage aiImage)
    {
        return new AIImageDto
        {
            AIImageId = aiImage.AiImageId,
            UserId = aiImage.UserId,
            Prompt = aiImage.Prompt,
            NegativePrompt = aiImage.NegativePrompt,
            ImageUrl = aiImage.ImageUrl,
            ThumbnailUrl = aiImage.ThumbnailUrl,
            Style = aiImage.Style,
            Width = aiImage.Width,
            Height = aiImage.Height,
            Status = (AIImageStatus)aiImage.Status,
            ErrorMessage = aiImage.ErrorMessage,
            CreatedAt = aiImage.CreatedAt,
            UpdatedAt = aiImage.UpdatedAt
        };
    }

    public async Task<AIImageDto?> GetAIImageByIdAsync(string aiImageId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting AI image by ID {ImageId}", aiImageId);
        
        var aiImage = await _unitOfWork.AIImageRepository.GetByIdAsync(aiImageId, cancellationToken);
        return aiImage == null ? null : MapToDto(aiImage);
    }

    public async Task<List<AIImageDto>> GetAIImagesByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting AI images for user {UserId}", userId);
        
        var aiImages = await _unitOfWork.AIImageRepository.GetByUserIdAsync(userId, cancellationToken);
        return aiImages.Select(MapToDto).ToList();
    }

    public async Task<AIImageDto> UpdateAIImageStatusAsync(string aiImageId, UpdateAIImageStatusDto updateDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating AI image status for ID {ImageId} to {Status}", aiImageId, updateDto.Status);

        var aiImage = await _unitOfWork.AIImageRepository.GetByIdAsync(aiImageId, cancellationToken);
        if (aiImage == null)
        {
            _logger.LogWarning("AI Image with ID {ImageId} not found", aiImageId);
            throw new NotFoundException(nameof(AiImage), aiImageId);
        }

        aiImage.Status = (int)updateDto.Status;
        aiImage.UpdatedAt = DateTime.Now;

        if (updateDto.ImageUrl != null)
            aiImage.ImageUrl = updateDto.ImageUrl;

        if (updateDto.ThumbnailUrl != null)
            aiImage.ThumbnailUrl = updateDto.ThumbnailUrl;

        if (updateDto.ErrorMessage != null)
            aiImage.ErrorMessage = updateDto.ErrorMessage;

        await _unitOfWork.AIImageRepository.UpdateAsync(aiImage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AI image status updated successfully for ID {ImageId}", aiImageId);
        return MapToDto(aiImage);
    }

    public async Task<bool> DeleteAIImageAsync(string aiImageId, string userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting AI image {ImageId} for user {UserId}", aiImageId, userId);

        var exists = await _unitOfWork.AIImageRepository.ExistsAsync(aiImageId, cancellationToken);
        if (!exists)
        {
            _logger.LogWarning("AI Image with ID {ImageId} not found", aiImageId);
            throw new NotFoundException(nameof(AiImage), aiImageId);
        }

        var result = await _unitOfWork.AIImageRepository.DeleteAsync(aiImageId, userId, cancellationToken);
        
        if (result)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("AI image {ImageId} deleted successfully", aiImageId);
        }
        else
        {
            _logger.LogWarning("User {UserId} does not have permission to delete AI image {ImageId}", userId, aiImageId);
            throw new UnauthorizedException("You do not have permission to delete this AI image");
        }

        return result;
    }

    
}
