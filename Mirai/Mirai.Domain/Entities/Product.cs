using System;
using System.Collections.Generic;

namespace Mirai.Domain.Entities;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? CategoryId { get; set; }

    public string? BrandId { get; set; }

    public bool IsActive { get; set; }

    public decimal? RatingAvg { get; set; }

    public int RatingCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
