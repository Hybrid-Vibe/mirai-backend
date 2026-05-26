namespace Mirai.Application.SearchFilter.Admin;

public class AdminOrderFilter
{
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public string? UserId { get; set; }
    public string? Search { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
