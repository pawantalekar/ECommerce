using System;
using System.Collections.Generic;

namespace Ecom.Domain.Entities;

public partial class Brand
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? LogoUrl { get; set; }
}
