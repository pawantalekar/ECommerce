using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Domain.Entities;

[Index("UserId", Name = "UK_Carts_UserId", IsUnique = true)]
public partial class Cart
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Cart")]
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
