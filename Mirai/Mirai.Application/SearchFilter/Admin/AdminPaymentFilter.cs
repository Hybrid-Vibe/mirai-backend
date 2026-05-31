namespace Mirai.Application.SearchFilter.Admin;

public class AdminPaymentFilter
{
    public int? Status { get; set; }
    public string? OrderId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
