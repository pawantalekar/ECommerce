namespace Ecom.Domain.Entities;

public partial class Review
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }

    public Guid OrderId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public Guid? ApprovedById { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public virtual Product Product { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual Order Order { get; set; } = null!;
    public virtual User? ApprovedBy { get; set; }
}
