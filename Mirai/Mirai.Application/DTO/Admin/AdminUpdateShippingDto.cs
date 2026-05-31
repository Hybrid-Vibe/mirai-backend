namespace Mirai.Application.DTO.Admin;

public class AdminUpdateShippingDto
{
    public int? ShippingStatus { get; set; }
    public string? Carrier { get; set; }
    public string? TrackingCode { get; set; }
    public decimal? ShippingFee { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}
