namespace Mirai.Application.DTO.Admin;

public class AdminRevenueChartDto
{
    public string Period { get; set; } = null!;
    public IReadOnlyList<AdminRevenueChartPointDto> Data { get; set; } = Array.Empty<AdminRevenueChartPointDto>();
}

public class AdminRevenueChartPointDto
{
    public string Label { get; set; } = null!;
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
}
