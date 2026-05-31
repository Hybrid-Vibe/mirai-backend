namespace Mirai.Application.DTO.Admin;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int PendingReviews { get; set; }
    public int TotalPayments { get; set; }
}
