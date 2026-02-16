using Ecom.Domain.Enums;

namespace Ecom.Domain.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public PaymentStatusEnum PaymentStatus { get; set; }

    public OrderStatusEnum OrderStatus { get; set; }

    public string ShippingFullName { get; set; } = null!;

    public string ShippingPhone { get; set; } = null!;

    public string ShippingAddressLine1 { get; set; } = null!;

    public string? ShippingAddressLine2 { get; set; }

    public string ShippingCity { get; set; } = null!;

    public string ShippingState { get; set; } = null!;

    public string ShippingPincode { get; set; } = null!;
    public string ShippingCountry { get; set; } = null!;

    public string ShippingAddressType { get; set; } = null!;

    public string? RazorpayPaymentId { get; set; }

    public string? RazorpayOrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public virtual User User { get; set; } = null!;
}
