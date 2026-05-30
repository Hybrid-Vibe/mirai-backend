using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Common;
using Mirai.Application.DTO;
using Mirai.Application.DTO.Admin;
using Mirai.Application.Interfaces.Services;
using Mirai.Application.SearchFilter.Admin;
using Mirai.Domain.Enum;

namespace Mirai.Controllers;

[Route("api/admin")]
[ApiController]
[Authorize(Roles = RoleIds.Admin)]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard/summary")]
    public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var summary = await _adminService.GetDashboardSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    [HttpGet("dashboard/revenue-chart")]
    public async Task<IActionResult> GetRevenueChart([FromQuery] string period = "week", CancellationToken cancellationToken = default)
    {
        var chart = await _adminService.GetRevenueChartAsync(period, cancellationToken);
        return Ok(chart);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] AdminUserFilter filter, CancellationToken cancellationToken)
    {
        var users = await _adminService.GetUsersAsync(filter, cancellationToken);
        return Ok(users);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserById(string userId, CancellationToken cancellationToken)
    {
        var user = await _adminService.GetUserByIdAsync(userId, cancellationToken);
        if (user == null)
            return NotFound($"User {userId} not found");
        return Ok(user);
    }

    [HttpPut("users/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] AdminUpdateUserDto dto, CancellationToken cancellationToken)
    {
        var user = await _adminService.UpdateUserAsync(userId, dto, cancellationToken);
        return Ok(user);
    }

    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(string userId, [FromBody] AdminUpdateUserRoleDto dto, CancellationToken cancellationToken)
    {
        var user = await _adminService.UpdateUserRoleAsync(userId, dto, cancellationToken);
        return Ok(user);
    }

    [HttpPut("users/{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus(string userId, [FromBody] AdminUpdateUserStatusDto dto, CancellationToken cancellationToken)
    {
        var user = await _adminService.UpdateUserStatusAsync(userId, dto, cancellationToken);
        return Ok(user);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders([FromQuery] AdminOrderFilter filter, CancellationToken cancellationToken)
    {
        var orders = await _adminService.GetOrdersAsync(filter, cancellationToken);
        return Ok(orders);
    }

    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrderDetail(string orderId, CancellationToken cancellationToken)
    {
        var order = await _adminService.GetOrderDetailAsync(orderId, cancellationToken);
        return Ok(order);
    }

    [HttpPut("orders/{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] OrderStatus newStatus, CancellationToken cancellationToken)
    {
        await _adminService.UpdateOrderStatusAsync(orderId, newStatus, cancellationToken);
        return Ok("Order status updated successfully");
    }

    [HttpPut("orders/{orderId}/payment-status")]
    public async Task<IActionResult> UpdateOrderPaymentStatus(string orderId, [FromBody] PaymentStatus newStatus, CancellationToken cancellationToken)
    {
        await _adminService.UpdateOrderPaymentStatusAsync(orderId, newStatus, cancellationToken);
        return Ok("Order payment status updated successfully");
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments([FromQuery] AdminPaymentFilter filter, CancellationToken cancellationToken)
    {
        var payments = await _adminService.GetPaymentsAsync(filter, cancellationToken);
        return Ok(payments);
    }

    [HttpPut("payments/{paymentId}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(string paymentId, [FromBody] PaymentStatusInPayment newStatus, CancellationToken cancellationToken)
    {
        await _adminService.UpdatePaymentStatusAsync(paymentId, newStatus, cancellationToken);
        return Ok("Payment status updated successfully");
    }

    [HttpGet("reviews")]
    public async Task<IActionResult> GetReviews([FromQuery] AdminReviewFilter filter, CancellationToken cancellationToken)
    {
        var reviews = await _adminService.GetReviewsAsync(filter, cancellationToken);
        return Ok(reviews);
    }

    [HttpPut("reviews/{reviewId}/approve")]
    public async Task<IActionResult> ApproveReview(string reviewId, CancellationToken cancellationToken)
    {
        await _adminService.ApproveReviewAsync(reviewId, cancellationToken);
        return Ok("Review approved successfully");
    }

    [HttpDelete("reviews/{reviewId}")]
    public async Task<IActionResult> DeleteReview(string reviewId, CancellationToken cancellationToken)
    {
        await _adminService.DeleteReviewAsync(reviewId, cancellationToken);
        return Ok("Review deleted successfully");
    }

    [HttpGet("shippings")]
    public async Task<IActionResult> GetShippings([FromQuery] AdminShippingFilter filter, CancellationToken cancellationToken)
    {
        var shippings = await _adminService.GetShippingsAsync(filter, cancellationToken);
        return Ok(shippings);
    }

    [HttpGet("shippings/{shippingId}")]
    public async Task<IActionResult> GetShippingById(string shippingId, CancellationToken cancellationToken)
    {
        var shipping = await _adminService.GetShippingByIdAsync(shippingId, cancellationToken);
        return Ok(shipping);
    }

    [HttpPost("shippings")]
    public async Task<IActionResult> CreateShipping([FromBody] AdminCreateShippingDto dto, CancellationToken cancellationToken)
    {
        var shipping = await _adminService.CreateShippingAsync(dto, cancellationToken);
        return Ok(shipping);
    }

    [HttpPut("shippings/{shippingId}")]
    public async Task<IActionResult> UpdateShipping(string shippingId, [FromBody] AdminUpdateShippingDto dto, CancellationToken cancellationToken)
    {
        var shipping = await _adminService.UpdateShippingAsync(shippingId, dto, cancellationToken);
        return Ok(shipping);
    }

    [HttpPut("products/{productId}/deactivate")]
    public async Task<IActionResult> DeactivateProduct(string productId, CancellationToken cancellationToken)
    {
        await _adminService.DeactivateProductAsync(productId, cancellationToken);
        return Ok("Product deactivated successfully");
    }

    [HttpPut("products/{productId}/activate")]
    public async Task<IActionResult> ActivateProduct(string productId, CancellationToken cancellationToken)
    {
        await _adminService.ActivateProductAsync(productId, cancellationToken);
        return Ok("Product activated successfully");
    }

    [HttpDelete("products/{productId}")]
    public async Task<IActionResult> DeleteProduct(string productId, CancellationToken cancellationToken)
    {
        await _adminService.DeleteProductAsync(productId, cancellationToken);
        return Ok("Product deleted successfully");
    }

    [HttpGet("ai-images")]
    public async Task<IActionResult> GetAIImages([FromQuery] AdminAIImageFilter filter, CancellationToken cancellationToken)
    {
        var images = await _adminService.GetAIImagesAsync(filter, cancellationToken);
        return Ok(images);
    }

    [HttpPut("ai-images/{aiImageId}/status")]
    public async Task<IActionResult> UpdateAIImageStatus(string aiImageId, [FromBody] UpdateAIImageStatusDto dto, CancellationToken cancellationToken)
    {
        var image = await _adminService.UpdateAIImageStatusAsync(aiImageId, dto, cancellationToken);
        return Ok(image);
    }

    [HttpDelete("ai-images/{aiImageId}")]
    public async Task<IActionResult> DeleteAIImage(string aiImageId, CancellationToken cancellationToken)
    {
        await _adminService.DeleteAIImageAsync(aiImageId, cancellationToken);
        return Ok("AI image deleted successfully");
    }
}
