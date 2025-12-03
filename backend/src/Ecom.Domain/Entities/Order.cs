namespace Ecom.Domain.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public string ShippingFullName { get; set; } = null!;

    public string ShippingPhone { get; set; } = null!;

    public string ShippingAddressLine1 { get; set; } = null!;

    public string? ShippingAddressLine2 { get; set; }

    public string ShippingCity { get; set; } = null!;

    public string ShippingState { get; set; } = null!;

    public string ShippingPincode { get; set; } = null!;

    public string? RazorpayPaymentId { get; set; }

    public string? RazorpayOrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
