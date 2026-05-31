namespace Mirai.Application.SearchFilter.Admin;

public class AdminReviewFilter
{
    public bool? IsApproved { get; set; }
    public string? ProductId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
