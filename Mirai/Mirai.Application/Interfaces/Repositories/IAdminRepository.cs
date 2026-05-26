using Mirai.Application.DTO;
using Mirai.Application.DTO.Admin;
using Mirai.Application.Extension;
using Mirai.Application.SearchFilter.Admin;

namespace Mirai.Application.Interfaces.Repositories;

public interface IAdminRepository
{
    Task<AdminDashboardDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<GetUserDto>> GetUsersPagedAsync(AdminUserFilter filter, CancellationToken cancellationToken = default);
    Task<GetUserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(string userId, AdminUpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
    Task<bool> RoleExistsAsync(string roleId, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminOrderListDto>> GetOrdersPagedAsync(AdminOrderFilter filter, CancellationToken cancellationToken = default);
    Task<AdminOrderDetailDto?> GetOrderDetailAsync(string orderId, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminPaymentDto>> GetPaymentsPagedAsync(AdminPaymentFilter filter, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminReviewDto>> GetReviewsPagedAsync(AdminReviewFilter filter, CancellationToken cancellationToken = default);
    Task<bool> ApproveReviewAsync(string reviewId, CancellationToken cancellationToken = default);
    Task<bool> DeleteReviewAsync(string reviewId, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminShippingDto>> GetShippingsPagedAsync(AdminShippingFilter filter, CancellationToken cancellationToken = default);
    Task<AdminShippingDto?> GetShippingByIdAsync(string shippingId, CancellationToken cancellationToken = default);
    Task<AdminShippingDto> CreateShippingAsync(AdminCreateShippingDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateShippingAsync(string shippingId, AdminUpdateShippingDto dto, CancellationToken cancellationToken = default);
    Task<bool> SetProductActiveAsync(string productId, bool isActive, CancellationToken cancellationToken = default);
}
