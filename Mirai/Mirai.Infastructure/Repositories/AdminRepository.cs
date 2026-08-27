using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.DTO.Admin;
using Mirai.Application.Extension;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.SearchFilter.Admin;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using Mirai.Infastructure.Data;

namespace Mirai.Infastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;

    public AdminRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var paidStatus = PaymentStatus.Paid;
        var pendingOrderStatuses = new[]
        {
            OrderStatus.Created,
            OrderStatus.Confirmed
        };

        return new AdminDashboardDto
        {
            TotalUsers = await _context.Users.CountAsync(cancellationToken),
            ActiveUsers = await _context.Users.CountAsync(u => u.IsActive, cancellationToken),
            TotalOrders = await _context.Orders.CountAsync(cancellationToken),
            PendingOrders = await _context.Orders.CountAsync(
                o => o.Status != null && pendingOrderStatuses.Contains((OrderStatus)o.Status),
                cancellationToken),
            TotalRevenue = await _context.Payments
                .Where(p => p.Status == (int)paidStatus)
                .SumAsync(p => p.Amount ?? 0, cancellationToken),
            TotalProducts = await _context.Products.CountAsync(cancellationToken),
            ActiveProducts = await _context.Products.CountAsync(p => p.IsActive, cancellationToken),
            PendingReviews = await _context.Reviews.CountAsync(r => !r.IsApproved, cancellationToken),
            TotalPayments = await _context.Payments.CountAsync(cancellationToken)
        };
    }

    public async Task<AdminRevenueChartDto> GetRevenueChartAsync(string period, CancellationToken cancellationToken = default)
    {
        var paidStatus = PaymentStatus.Paid;
        var isMonth = string.Equals(period, "month", StringComparison.OrdinalIgnoreCase);
        var normalizedPeriod = isMonth ? "month" : "week";
        var today = DateTime.Now.Date;

        if (isMonth)
        {
            var rangeStart = today.AddMonths(-11);
            var rangeEnd = today.AddDays(1);

            var revenueByMonth = await _context.Orders
                .Where(o => o.PaymentStatus == (int)paidStatus && o.CreatedAt >= rangeStart && o.CreatedAt < rangeEnd)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync(cancellationToken);

            var points = new List<AdminRevenueChartPointDto>();
            for (var i = 11; i >= 0; i--)
            {
                var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(-i);
                var revenue = revenueByMonth
                    .FirstOrDefault(x => x.Year == monthStart.Year && x.Month == monthStart.Month)
                    ?.Revenue ?? 0m;

                points.Add(new AdminRevenueChartPointDto
                {
                    Date = monthStart,
                    Label = monthStart.ToString("yyyy-MM"),
                    Revenue = revenue
                });
            }

            return new AdminRevenueChartDto { Period = normalizedPeriod, Data = points };
        }

        var weekStart = today.AddDays(-6);
        var weekEnd = today.AddDays(1);

        var revenueByDay = await _context.Orders
            .Where(o => o.PaymentStatus == (int)paidStatus && o.CreatedAt >= weekStart && o.CreatedAt < weekEnd)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount) })
            .ToListAsync(cancellationToken);

        var weekPoints = new List<AdminRevenueChartPointDto>();
        for (var day = weekStart; day <= today; day = day.AddDays(1))
        {
            var revenue = revenueByDay.FirstOrDefault(x => x.Date == day)?.Revenue ?? 0m;
            weekPoints.Add(new AdminRevenueChartPointDto
            {
                Date = day,
                Label = day.ToString("yyyy-MM-dd"),
                Revenue = revenue
            });
        }

        return new AdminRevenueChartDto { Period = normalizedPeriod, Data = weekPoints };
    }

    public async Task<PagedResult<GetUserDto>> GetUsersPagedAsync(AdminUserFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(u =>
                u.Email.Contains(search) ||
                (u.FullName != null && u.FullName.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(filter.RoleId))
            query = query.Where(u => u.RoleId == filter.RoleId);

        if (filter.IsActive.HasValue)
            query = query.Where(u => u.IsActive == filter.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(u => new GetUserDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                RoleId = u.RoleId,
                RoleName = u.Role.RoleName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<GetUserDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public Task<GetUserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return _context.Users
            .Where(u => u.UserId == userId)
            .Select(u => new GetUserDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                RoleId = u.RoleId,
                RoleName = u.Role.RoleName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> UpdateUserAsync(string userId, AdminUpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (user == null) return false;

        if (dto.FullName != null) user.FullName = dto.FullName;
        if (dto.Phone != null) user.Phone = dto.Phone;
        user.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (user == null) return false;

        user.RoleId = roleId;
        user.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateUserStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (user == null) return false;

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> RoleExistsAsync(string roleId, CancellationToken cancellationToken = default)
    {
        return _context.Roles.AnyAsync(r => r.RoleId == roleId, cancellationToken);
    }

    public async Task<PagedResult<AdminOrderListDto>> GetOrdersPagedAsync(AdminOrderFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(o => o.Status == (int)filter.Status.Value);

        if (filter.PaymentStatus.HasValue)
            query = query.Where(o => o.PaymentStatus == (int)filter.PaymentStatus.Value);
        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(o => o.UserId == filter.UserId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(o =>
                (o.OrderNumber != null && o.OrderNumber.Contains(search)) ||
                o.OrderId.Contains(search));
        }

        if (filter.FromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(o => o.CreatedAt <= filter.ToDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(o => new AdminOrderListDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                UserId = o.UserId,
                UserEmail = o.User.Email,
                UserFullName = o.User.FullName,
                TotalAmount = o.TotalAmount,
                Status = (OrderStatus)o.Status,
                PaymentStatus = (PaymentStatus)o.PaymentStatus,
                CreatedAt = o.CreatedAt,
                ItemCount = o.OrderItems.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminOrderListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<AdminOrderDetailDto?> GetOrderDetailAsync(string orderId, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.Shippings)
            .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

        if (order == null) return null;

        return new AdminOrderDetailDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            UserEmail = order.User.Email,
            UserFullName = order.User.FullName,
            OrderNumber = order.OrderNumber,
            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            ShippingFee = order.ShippingFee,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            Status = (OrderStatus)order.Status,
            PaymentStatus = (PaymentStatus)order.PaymentStatus,
            Note = order.Note,
            CreatedAt = order.CreatedAt,
            PlacedAt = order.PlacedAt,
            UpdatedAt = order.UpdatedAt,
            CancelledAt = order.CancelledAt,
            Items = order.OrderItems.Select(i => new OrderItemResponseDto
            {
                ProductName = i.ProductName,
                VariantName = i.VariantName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice ?? 0,
                Price = i.Price ?? 0
            }).ToList(),
            Payments = order.Payments.Select(p => new AdminPaymentDto
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId,
                OrderNumber = order.OrderNumber,
                Method = p.Method,
                Provider = p.Provider,
                Status = p.Status,
                Amount = p.Amount,
                TransactionId = p.TransactionId,
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt
            }).ToList(),
            Shippings = order.Shippings.Select(s => new AdminShippingDto
            {
                ShippingId = s.ShippingId,
                OrderId = s.OrderId,
                OrderNumber = order.OrderNumber,
                AddressId = s.AddressId,
                ShippingStatus = s.ShippingStatus,
                Carrier = s.Carrier,
                TrackingCode = s.TrackingCode,
                ShippingFee = s.ShippingFee,
                ShippedAt = s.ShippedAt,
                DeliveredAt = s.DeliveredAt,
                CreatedAt = s.CreatedAt
            }).ToList()
        };
    }

    public async Task<PagedResult<AdminPaymentDto>> GetPaymentsPagedAsync(AdminPaymentFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Payments
            .Include(p => p.Order)
            .AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.OrderId))
            query = query.Where(p => p.OrderId == filter.OrderId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new AdminPaymentDto
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId,
                OrderNumber = p.Order.OrderNumber,
                Method = p.Method,
                Provider = p.Provider,
                Status = p.Status,
                Amount = p.Amount,
                TransactionId = p.TransactionId,
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminPaymentDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<PagedResult<AdminReviewDto>> GetReviewsPagedAsync(AdminReviewFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .AsQueryable();

        if (filter.IsApproved.HasValue)
            query = query.Where(r => r.IsApproved == filter.IsApproved.Value);

        if (!string.IsNullOrWhiteSpace(filter.ProductId))
            query = query.Where(r => r.ProductId == filter.ProductId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(r => new AdminReviewDto
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                UserEmail = r.User.Email,
                ProductId = r.ProductId,
                ProductName = r.Product.Name,
                VariantId = r.VariantId,
                Rating = r.Rating,
                Title = r.Title,
                Comment = r.Comment,
                IsApproved = r.IsApproved,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminReviewDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<bool> ApproveReviewAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId, cancellationToken);
        if (review == null) return false;

        review.IsApproved = true;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteReviewAsync(string reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId, cancellationToken);
        if (review == null) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResult<AdminShippingDto>> GetShippingsPagedAsync(AdminShippingFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Shippings
            .Include(s => s.Order)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.OrderId))
            query = query.Where(s => s.OrderId == filter.OrderId);

        if (filter.ShippingStatus.HasValue)
            query = query.Where(s => s.ShippingStatus == filter.ShippingStatus.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(s => new AdminShippingDto
            {
                ShippingId = s.ShippingId,
                OrderId = s.OrderId,
                OrderNumber = s.Order.OrderNumber,
                AddressId = s.AddressId,
                ShippingStatus = s.ShippingStatus,
                Carrier = s.Carrier,
                TrackingCode = s.TrackingCode,
                ShippingFee = s.ShippingFee,
                ShippedAt = s.ShippedAt,
                DeliveredAt = s.DeliveredAt,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AdminShippingDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<AdminShippingDto?> GetShippingByIdAsync(string shippingId, CancellationToken cancellationToken = default)
    {
        return await _context.Shippings
            .Include(s => s.Order)
            .Where(s => s.ShippingId == shippingId)
            .Select(s => new AdminShippingDto
            {
                ShippingId = s.ShippingId,
                OrderId = s.OrderId,
                OrderNumber = s.Order.OrderNumber,
                AddressId = s.AddressId,
                ShippingStatus = s.ShippingStatus,
                Carrier = s.Carrier,
                TrackingCode = s.TrackingCode,
                ShippingFee = s.ShippingFee,
                ShippedAt = s.ShippedAt,
                DeliveredAt = s.DeliveredAt,
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AdminShippingDto> CreateShippingAsync(AdminCreateShippingDto dto, CancellationToken cancellationToken = default)
    {
        var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == dto.OrderId, cancellationToken);
        if (!orderExists)
            throw new InvalidOperationException($"Order {dto.OrderId} not found");

        var addressExists = await _context.Addresses.AnyAsync(a => a.AddressId == dto.AddressId, cancellationToken);
        if (!addressExists)
            throw new InvalidOperationException($"Address {dto.AddressId} not found");

        var shipping = new Shipping
        {
            ShippingId = Guid.NewGuid().ToString(),
            OrderId = dto.OrderId,
            AddressId = dto.AddressId,
            ShippingStatus = dto.ShippingStatus ?? 1,
            Carrier = dto.Carrier,
            TrackingCode = dto.TrackingCode,
            ShippingFee = dto.ShippingFee,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Shippings.AddAsync(shipping, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetShippingByIdAsync(shipping.ShippingId, cancellationToken))!;
    }

    public async Task<bool> UpdateShippingAsync(string shippingId, AdminUpdateShippingDto dto, CancellationToken cancellationToken = default)
    {
        var shipping = await _context.Shippings.FirstOrDefaultAsync(s => s.ShippingId == shippingId, cancellationToken);
        if (shipping == null) return false;

        if (dto.ShippingStatus != null) shipping.ShippingStatus = dto.ShippingStatus;
        if (dto.Carrier != null) shipping.Carrier = dto.Carrier;
        if (dto.TrackingCode != null) shipping.TrackingCode = dto.TrackingCode;
        if (dto.ShippingFee.HasValue) shipping.ShippingFee = dto.ShippingFee;
        if (dto.ShippedAt.HasValue) shipping.ShippedAt = dto.ShippedAt;
        if (dto.DeliveredAt.HasValue) shipping.DeliveredAt = dto.DeliveredAt;
        shipping.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetProductActiveAsync(string productId, bool isActive, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
        if (product == null) return false;

        product.IsActive = isActive;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> DeleteProductAsync(string productId, CancellationToken cancellationToken = default)
        => SetProductActiveAsync(productId, false, cancellationToken);
}
