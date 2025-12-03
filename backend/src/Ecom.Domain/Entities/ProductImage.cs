using System;
using System.Collections.Generic;

namespace Ecom.Domain.Entities;

public partial class ProductImage
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Url { get; set; } = null!;

    public string? AltText { get; set; }

    public int? SortOrder { get; set; }

    public bool? IsThumbnail { get; set; }

    public virtual Product Product { get; set; } = null!;
}
