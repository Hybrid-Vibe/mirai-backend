namespace Mirai.Application.DTO.Admin;

public class AdminReviewDto
{
    public string ReviewId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string? UserEmail { get; set; }
    public string ProductId { get; set; } = null!;
    public string? ProductName { get; set; }
    public string? VariantId { get; set; }
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}
