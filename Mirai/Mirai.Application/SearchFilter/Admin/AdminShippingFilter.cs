namespace Mirai.Application.SearchFilter.Admin;

public class AdminShippingFilter
{
    public string? OrderId { get; set; }
    public string? ShippingStatus { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
