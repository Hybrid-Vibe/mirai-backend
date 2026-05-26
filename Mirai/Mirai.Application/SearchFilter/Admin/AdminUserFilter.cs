namespace Mirai.Application.SearchFilter.Admin;

public class AdminUserFilter
{
    public string? Search { get; set; }
    public string? RoleId { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
