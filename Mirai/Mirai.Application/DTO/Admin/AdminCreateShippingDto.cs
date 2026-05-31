namespace Mirai.Application.DTO.Admin;

public class AdminCreateShippingDto
{
    public string OrderId { get; set; } = null!;
    public string AddressId { get; set; } = null!;
    public int? ShippingStatus { get; set; }
    public string? Carrier { get; set; }
    public string? TrackingCode { get; set; }
    public decimal? ShippingFee { get; set; }
}
