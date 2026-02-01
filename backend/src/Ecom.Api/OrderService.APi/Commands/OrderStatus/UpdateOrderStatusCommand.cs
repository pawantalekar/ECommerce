using MediatR;

namespace OrderService.APi.Commands.OrderStatus
{
    public record UpdateOrderStatusCommand(
        string OrderNumber,
        string NewStatus,
        string? Notes
    ) : IRequest<UpdateOrderStatusResult>;
}
