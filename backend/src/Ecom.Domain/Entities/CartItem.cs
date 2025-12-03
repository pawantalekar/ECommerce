using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecom.Domain.Entities;

[Index("ProductId", Name = "IX_CartItems_ProductId")]
[Index("CartId", "ProductId", Name = "UK_CartItems_CartId_ProductId", IsUnique = true)]
public partial class CartItem
{
    [Key]
    public Guid Id { get; set; }

    public Guid CartId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("Items")]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;
}
