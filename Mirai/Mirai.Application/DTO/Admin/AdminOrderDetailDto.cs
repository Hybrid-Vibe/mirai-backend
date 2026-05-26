using Mirai.Application.DTO;

namespace Mirai.Application.DTO.Admin;

public class AdminOrderDetailDto
{
    public string OrderId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string? UserEmail { get; set; }
    public string? UserFullName { get; set; }
    public string? OrderNumber { get; set; }
    public decimal? Subtotal { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? ShippingFee { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PlacedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
    public List<AdminPaymentDto> Payments { get; set; } = new();
    public List<AdminShippingDto> Shippings { get; set; } = new();
}
