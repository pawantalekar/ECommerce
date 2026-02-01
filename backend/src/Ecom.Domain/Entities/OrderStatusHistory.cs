namespace Ecom.Domain.Entities;

public partial class OrderStatusHistory
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public Guid? ChangedByUserId { get; set; }

    public string? Notes { get; set; }
    public virtual Order? Order { get; set; }
}
