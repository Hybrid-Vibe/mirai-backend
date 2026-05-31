using Mirai.Domain.Enums;

namespace Mirai.Application.SearchFilter.Admin;

public class AdminAIImageFilter
{
    public string? UserId { get; set; }
    public AIImageStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
