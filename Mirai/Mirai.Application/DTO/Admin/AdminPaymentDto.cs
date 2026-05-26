namespace Mirai.Application.DTO.Admin;

public class AdminPaymentDto
{
    public string PaymentId { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public string? OrderNumber { get; set; }
    public string? Method { get; set; }
    public string? Provider { get; set; }
    public string? Status { get; set; }
    public decimal? Amount { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
