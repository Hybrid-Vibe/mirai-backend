using Mirai.Application.DTO;
using Mirai.Application.DTO.Admin;
using Mirai.Application.Extension;
using Mirai.Application.SearchFilter.Admin;
using Mirai.Domain.Enum;
using Mirai.Domain.Enums;

namespace Mirai.Application.Interfaces.Services;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task<AdminRevenueChartDto> GetRevenueChartAsync(string period, CancellationToken cancellationToken = default);
    Task<PagedResult<GetUserDto>> GetUsersAsync(AdminUserFilter filter, CancellationToken cancellationToken = default);
    Task<GetUserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<GetUserDto> UpdateUserAsync(string userId, AdminUpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<GetUserDto> UpdateUserRoleAsync(string userId, AdminUpdateUserRoleDto dto, CancellationToken cancellationToken = default);
    Task<GetUserDto> UpdateUserStatusAsync(string userId, AdminUpdateUserStatusDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminOrderListDto>> GetOrdersAsync(AdminOrderFilter filter, CancellationToken cancellationToken = default);
    Task<AdminOrderDetailDto> GetOrderDetailAsync(string orderId, CancellationToken cancellationToken = default);
    Task UpdateOrderStatusAsync(string orderId, OrderStatus newStatus, CancellationToken cancellationToken = default);
    Task UpdateOrderPaymentStatusAsync(string orderId, PaymentStatus newStatus, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminPaymentDto>> GetPaymentsAsync(AdminPaymentFilter filter, CancellationToken cancellationToken = default);
    Task UpdatePaymentStatusAsync(string paymentId, PaymentStatusInPayment newStatus, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminReviewDto>> GetReviewsAsync(AdminReviewFilter filter, CancellationToken cancellationToken = default);
    Task ApproveReviewAsync(string reviewId, CancellationToken cancellationToken = default);
    Task DeleteReviewAsync(string reviewId, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminShippingDto>> GetShippingsAsync(AdminShippingFilter filter, CancellationToken cancellationToken = default);
    Task<AdminShippingDto> GetShippingByIdAsync(string shippingId, CancellationToken cancellationToken = default);
    Task<AdminShippingDto> CreateShippingAsync(AdminCreateShippingDto dto, CancellationToken cancellationToken = default);
    Task<AdminShippingDto> UpdateShippingAsync(string shippingId, AdminUpdateShippingDto dto, CancellationToken cancellationToken = default);
    Task DeactivateProductAsync(string productId, CancellationToken cancellationToken = default);
    Task ActivateProductAsync(string productId, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(string productId, CancellationToken cancellationToken = default);
    Task<PagedResult<AIImageDto>> GetAIImagesAsync(AdminAIImageFilter filter, CancellationToken cancellationToken = default);
    Task<AIImageDto> UpdateAIImageStatusAsync(string aiImageId, UpdateAIImageStatusDto dto, CancellationToken cancellationToken = default);
    Task DeleteAIImageAsync(string aiImageId, CancellationToken cancellationToken = default);
}
