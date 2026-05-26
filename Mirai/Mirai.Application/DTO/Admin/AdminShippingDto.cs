namespace Mirai.Application.DTO.Admin;

public class AdminShippingDto
{
    public string ShippingId { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public string? OrderNumber { get; set; }
    public string AddressId { get; set; } = null!;
    public string? ShippingStatus { get; set; }
    public string? Carrier { get; set; }
    public string? TrackingCode { get; set; }
    public decimal? ShippingFee { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
