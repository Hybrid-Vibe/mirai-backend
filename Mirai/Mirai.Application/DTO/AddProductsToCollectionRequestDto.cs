using System;
using System.Collections.Generic;

namespace Mirai.Application.DTO;

public class AddProductsToCollectionRequestDto
{
    public Guid CollectionId { get; set; }
    public List<string> ProductIds { get; set; } = new List<string>();
}
