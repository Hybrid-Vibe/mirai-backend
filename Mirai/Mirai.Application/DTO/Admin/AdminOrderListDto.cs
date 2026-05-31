namespace Mirai.Application.DTO.Admin;

public class AdminOrderListDto
{
    public string OrderId { get; set; } = null!;
    public string? OrderNumber { get; set; }
    public string UserId { get; set; } = null!;
    public string? UserEmail { get; set; }
    public string? UserFullName { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemCount { get; set; }
}
