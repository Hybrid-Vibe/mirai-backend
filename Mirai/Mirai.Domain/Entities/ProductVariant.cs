using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class ProductVariant
{
    public string VariantId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string? Color { get; set; }

    public string? PhoneModel { get; set; }

    public decimal? Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public decimal? CostPrice { get; set; }

    public string? Barcode { get; set; }

    public int? Stock { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<FlashSaleItem> FlashSaleItems { get; set; } = new List<FlashSaleItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
