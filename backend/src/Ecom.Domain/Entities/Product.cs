using System;
using System.Collections.Generic;

namespace Ecom.Domain.Entities;

public partial class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    //if poduct name is Iphone 16 pro slug will be Iphone-16-pro which will be readable and willll be easy for the search
    public string Slug { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string Sku { get; set; } = null!;

    public int StockQuantity { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? BrandId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsFeatured { get; set; }

    public DateTime? CreatedAt { get; set; }
    public virtual Brand? Brand { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();


    
    public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}
