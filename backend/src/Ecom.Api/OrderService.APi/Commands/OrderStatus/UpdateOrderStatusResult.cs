namespace OrderService.APi.Commands.OrderStatus
{
    public record UpdateOrderStatusResult(
        string OrderNumber,
        string Status,
        DateTime ChangedAt
    );
}
