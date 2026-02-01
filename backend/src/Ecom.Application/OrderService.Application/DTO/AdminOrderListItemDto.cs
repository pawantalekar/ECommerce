public record AdminOrderListItemDto(
    string OrderNumber,
    string CustomerName,
    string email,
    DateTime CreatedAt,
    decimal TotalAmount,
    string OrderStatus,
    string PaymentStatus
);