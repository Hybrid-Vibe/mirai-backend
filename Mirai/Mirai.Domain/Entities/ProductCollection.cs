using System;

namespace Mirai.Domain.Entities;

public class ProductCollection
{
    public string ProductId { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;

    public Guid CollectionId { get; set; }
    public virtual Collection Collection { get; set; } = null!;
}
