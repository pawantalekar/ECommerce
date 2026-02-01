namespace OrderService.APi.Commands.OrderStatus
{
    public record UpdateOrderStatusRequest(string Status, string? Notes);
}
