namespace Mirai.Application.SearchFilter.Admin;

public class AdminShippingFilter
{
    public string? OrderId { get; set; }
    public int? ShippingStatus { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
