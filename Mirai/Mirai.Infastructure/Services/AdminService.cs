using Mirai.Application.Common;
using Mirai.Application.Common.Exceptions;
using Mirai.Application.DTO;
using Mirai.Application.DTO.Admin;
using Mirai.Application.Extension;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Application.SearchFilter.Admin;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using Mirai.Domain.Enums;

namespace Mirai.Infastructure.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIImageService _aiImageService;

    public AdminService(IUnitOfWork unitOfWork, IAIImageService aiImageService)
    {
        _unitOfWork = unitOfWork;
        _aiImageService = aiImageService;
    }

    public Task<AdminDashboardDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        => _unitOfWork.AdminRepository.GetDashboardSummaryAsync(cancellationToken);

    public Task<PagedResult<GetUserDto>> GetUsersAsync(AdminUserFilter filter, CancellationToken cancellationToken = default)
        => _unitOfWork.AdminRepository.GetUsersPagedAsync(filter, cancellationToken);

    public Task<GetUserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        => _unitOfWork.AdminRepository.GetUserByIdAsync(userId, cancellationToken);

    public async Task<GetUserDto> UpdateUserAsync(string userId, AdminUpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _unitOfWork.AdminRepository.UpdateUserAsync(userId, dto, cancellationToken);
        if (!updated)
            throw new NotFoundException(nameof(User), userId);

        return (await GetUserByIdAsync(userId, cancellationToken))!;
    }

    public async Task<GetUserDto> UpdateUserRoleAsync(string userId, AdminUpdateUserRoleDto dto, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.AdminRepository.RoleExistsAsync(dto.RoleId, cancellationToken))
            throw new BadRequestException($"Role '{dto.RoleId}' does not exist");

        if (dto.RoleId is not (RoleIds.Admin or RoleIds.Staff or RoleIds.Customer))
            throw new BadRequestException("RoleId must be 1 (Admin), 2 (Staff), or 3 (Customer)");

        var updated = await _unitOfWork.AdminRepository.UpdateUserRoleAsync(userId, dto.RoleId, cancellationToken);
        if (!updated)
            throw new NotFoundException(nameof(User), userId);

        return (await GetUserByIdAsync(userId, cancellationToken))!;
    }

    public async Task<GetUserDto> UpdateUserStatusAsync(string userId, AdminUpdateUserStatusDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _unitOfWork.AdminRepository.UpdateUserStatusAsync(userId, dto.IsActive, cancellationToken);
        if (!updated)
            throw new NotFoundException(nameof(User), userId);

        return (await GetUserByIdAsync(userId, cancellationToken))!;
    }

    public Task<PagedResult<AdminOrderListDto>> GetOrdersAsync(AdminOrderFilter filter, CancellationToken cancellationToken = default)
        => _unitOfWork.AdminRepository.GetOrdersPagedAsync(filter, cancellationToken);

    public async Task<AdminOrderDetailDto> GetOrderDetailAsync(string orderId, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.AdminRepository.GetOrderDetailAsync(orderId, cancellationToken);
        if (order == null)
            throw new NotFoundException(nameof(Order), orderId);
        return order;
    }

    public async Task UpdateOrderStatusAsync(string orderId, OrderStatus newStatus, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new NotFoundException(nameof(Order), orderId);

        await _unitOfWork.OrderRepository.UpdateOrderStatus(orderId, newStatus);
    }

    public async Task UpdateOrderPaymentStatusAsync(string orderId, PaymentStatus newStatus, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new NotFoundException(nameof(Order), orderId);

        await _unitOfWork.OrderRepository.UpdatePaymentStatus(orderId, newStatus);
    }

    public Task<PagedResult<AdminPaymentDto>> GetPaymentsAsync(AdminPaymentFilter filter, CancellationToken cancellationToken = default)
        => _unitOfWork.AdminRepository.GetPaymentsPagedAsync(filter, cancellationToken);

    public async Task UpdatePaymentStatusAsync(string paymentId, PaymentStatusInPayment newStatus, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
        if (payment == null)
            throw new NotFoundException(nameof(Payment), paymentId);

        await _unitOfWork.PaymentRepository.UpdatePaymentStatusByPaymentId(paymentId, newStatus);
    }

    public Task<PagedResult<AdminReviewDto>> GetReviewsAsync(AdminReviewFilter filter, CancellationToken cancellationToken = default)
        => _unitOfWork.AdminRepository.GetReviewsPagedAsync(filter, cancellationToken);

    public async Task ApproveReviewAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        var approved = await _unitOfWork.AdminRepository.ApproveReviewAsync(reviewId, cancellationToken);
        if (!approved)
            throw new NotFoundException(nameof(Review), reviewId);
    }

    public async Task DeleteReviewAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        var deleted = await _unitOfWork.AdminRepository.DeleteReviewAsync(reviewId, cancellationToken);
        if (!deleted)
            throw new NotFoundException(nameof(Review), reviewId);
    }

    public Task<PagedResult<AdminShippingDto>> GetShippingsAsync(AdminShippingFilter filter, CancellationToken cancellationToken = default)
        => _unitOfWork.AdminRepository.GetShippingsPagedAsync(filter, cancellationToken);

    public async Task<AdminShippingDto> GetShippingByIdAsync(string shippingId, CancellationToken cancellationToken = default)
    {
        var shipping = await _unitOfWork.AdminRepository.GetShippingByIdAsync(shippingId, cancellationToken);
        if (shipping == null)
            throw new NotFoundException(nameof(Shipping), shippingId);
        return shipping;
    }

    public Task<AdminShippingDto> CreateShippingAsync(AdminCreateShippingDto dto, CancellationToken cancellationToken = default)
        => _unitOfWork.AdminRepository.CreateShippingAsync(dto, cancellationToken);

    public async Task<AdminShippingDto> UpdateShippingAsync(string shippingId, AdminUpdateShippingDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _unitOfWork.AdminRepository.UpdateShippingAsync(shippingId, dto, cancellationToken);
        if (!updated)
            throw new NotFoundException(nameof(Shipping), shippingId);

        return await GetShippingByIdAsync(shippingId, cancellationToken);
    }

    public async Task DeactivateProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        var updated = await _unitOfWork.AdminRepository.SetProductActiveAsync(productId, false, cancellationToken);
        if (!updated)
            throw new NotFoundException(nameof(Product), productId);
    }

    public async Task ActivateProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        var updated = await _unitOfWork.AdminRepository.SetProductActiveAsync(productId, true, cancellationToken);
        if (!updated)
            throw new NotFoundException(nameof(Product), productId);
    }

    public async Task<PagedResult<AIImageDto>> GetAIImagesAsync(AdminAIImageFilter filter, CancellationToken cancellationToken = default)
    {
        var paged = await _unitOfWork.AIImageRepository.GetAllPagedAsync(filter, cancellationToken);
        return new PagedResult<AIImageDto>
        {
            Items = paged.Items.Select(MapAIImage).ToList(),
            TotalCount = paged.TotalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize
        };
    }

    public Task<AIImageDto> UpdateAIImageStatusAsync(string aiImageId, UpdateAIImageStatusDto dto, CancellationToken cancellationToken = default)
        => _aiImageService.UpdateAIImageStatusAsync(aiImageId, dto, cancellationToken);

    public async Task DeleteAIImageAsync(string aiImageId, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.AIImageRepository.ExistsAsync(aiImageId, cancellationToken);
        if (!exists)
            throw new NotFoundException(nameof(AIImage), aiImageId);

        var deleted = await _unitOfWork.AIImageRepository.AdminDeleteAsync(aiImageId, cancellationToken);
        if (deleted)
            await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static AIImageDto MapAIImage(AIImage aiImage) => new()
    {
        AIImageId = aiImage.AIImageId,
        UserId = aiImage.UserId,
        Prompt = aiImage.Prompt,
        NegativePrompt = aiImage.NegativePrompt,
        ImageUrl = aiImage.ImageUrl,
        ThumbnailUrl = aiImage.ThumbnailUrl,
        Style = aiImage.Style,
        Width = aiImage.Width,
        Height = aiImage.Height,
        Status = aiImage.Status,
        ErrorMessage = aiImage.ErrorMessage,
        CreatedAt = aiImage.CreatedAt,
        UpdatedAt = aiImage.UpdatedAt
    };
}
